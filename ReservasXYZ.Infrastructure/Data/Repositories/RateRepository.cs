using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Interfaces;
using ReservasXYZ.Infrastructure.Data.Context;

namespace ReservasXYZ.Infrastructure.Data.Repositories;

public class RateRepository : Repository<Rate>, IRateRepository
{
    public RateRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Rate>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Room)
            .Include(r => r.Season)
            .OrderBy(r => r.Room.RoomNumber)
            .ThenBy(r => r.Season.StartDate)
            .ToListAsync();
    }

    public override async Task<Rate?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(r => r.Room)
            .Include(r => r.Season)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<decimal> GetRateForRoomAsync(int roomId, DateTime date)
    {
        var rate = await _dbSet
            .Include(r => r.Season)
            .Where(r => r.RoomId == roomId
                && r.IsActive
                && r.Season.IsActive
                && r.Season.StartDate <= date
                && r.Season.EndDate >= date)
            .FirstOrDefaultAsync();

        if (rate != null)
            return rate.PricePerNight;

        var room = await _context.Set<Room>().FindAsync(roomId);
        return room?.BasePrice ?? 0;
    }

    public async Task<decimal> CalculateTotalRateAsync(int roomId, DateTime checkIn, DateTime checkOut, int totalGuests = 1)
    {
        if (totalGuests < 1) totalGuests = 1;

        var totalFromProcedure = await CalculateTotalRateFromStoredProcedureAsync(roomId, checkIn, checkOut, totalGuests);
        if (totalFromProcedure.HasValue)
        {
            return totalFromProcedure.Value;
        }

        decimal total = 0;
        for (var date = checkIn; date < checkOut; date = date.AddDays(1))
        {
            total += await GetDailyRateWithGuestsAsync(roomId, date, totalGuests);
        }
        return total;
    }

    private async Task<decimal> GetDailyRateWithGuestsAsync(int roomId, DateTime date, int totalGuests)
    {
        var isMondayToThursday = date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Thursday;

        var candidates = await _dbSet
            .Include(r => r.Season)
            .Where(r => r.RoomId == roomId
                        && r.IsActive
                        && r.Season.IsActive
                        && r.Season.StartDate <= date
                        && r.Season.EndDate >= date)
            .ToListAsync();

        if (candidates.Count == 0)
        {
            var room = await _context.Set<Room>().FindAsync(roomId);
            return room?.BasePrice ?? 0;
        }

        var rate = candidates
            .Select(c => new
            {
                Rate = c,
                Score = isMondayToThursday
                        && c.Kind == ReservasXYZ.Domain.Enums.RateKind.SpecialWeekday
                        && c.Season.PriceMultiplier <= 1m ? 0 : 1
            })
            .OrderBy(x => x.Score)
            .ThenByDescending(x => x.Rate.Season.PriceMultiplier)
            .First()
            .Rate;

        var multiplier = rate.Season.PriceMultiplier > 0 ? rate.Season.PriceMultiplier : 1m;
        var extraGuests = Math.Max(0, totalGuests - Math.Max(1, rate.BaseGuests));
        return (rate.PricePerNight * multiplier) + (extraGuests * rate.ExtraPersonPrice);
    }

    public async Task<IEnumerable<Rate>> GetRatesByRoomAsync(int roomId)
    {
        return await _dbSet
            .Include(r => r.Room)
            .Include(r => r.Season)
            .Where(r => r.RoomId == roomId)
            .ToListAsync();
    }

    private async Task<decimal?> CalculateTotalRateFromStoredProcedureAsync(int roomId, DateTime checkIn, DateTime checkOut, int totalGuests)
    {
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State == System.Data.ConnectionState.Closed;

        if (shouldClose)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "sp_CalculateTotalRate";
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@RoomId", roomId));
        command.Parameters.Add(new SqlParameter("@CheckIn", checkIn.Date));
        command.Parameters.Add(new SqlParameter("@CheckOut", checkOut.Date));
        command.Parameters.Add(new SqlParameter("@TotalGuests", totalGuests));

        var totalRateParameter = new SqlParameter("@TotalRate", System.Data.SqlDbType.Decimal)
        {
            Direction = System.Data.ParameterDirection.Output,
            Precision = 18,
            Scale = 2
        };
        command.Parameters.Add(totalRateParameter);

        await command.ExecuteNonQueryAsync();

        if (shouldClose)
        {
            await connection.CloseAsync();
        }

        return totalRateParameter.Value is decimal total ? total : null;
    }
}
