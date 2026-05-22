using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Enums;
using ReservasXYZ.Domain.Interfaces;
using ReservasXYZ.Infrastructure.Data.Context;

namespace ReservasXYZ.Infrastructure.Data.Repositories;

public class RoomRepository : Repository<Room>, IRoomRepository
{
    public RoomRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Accommodation)
                .ThenInclude(a => a.Site)
            .OrderBy(r => r.Accommodation.Site.Name)
            .ThenBy(r => r.Accommodation.Name)
            .ThenBy(r => r.RoomNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        var roomIds = await GetAvailableRoomIdsAsync("sp_GetAvailableRoomsByDates",
            new SqlParameter("@CheckIn", checkIn.Date),
            new SqlParameter("@CheckOut", checkOut.Date));

        if (roomIds.Count > 0)
        {
            return await GetRoomsByIdsAsync(roomIds);
        }

        var bookedRoomIds = await _context.Set<ReservationDetail>()
            .Where(rd => rd.Reservation.Status != ReservationStatus.Cancelled
                && rd.Reservation.CheckIn < checkOut
                && rd.Reservation.CheckOut > checkIn)
            .Select(rd => rd.RoomId)
            .Distinct()
            .ToListAsync();

        return await _dbSet
            .Include(r => r.Accommodation)
                .ThenInclude(a => a.Site)
            .Where(r => r.IsActive && !bookedRoomIds.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsByGuestsAsync(DateTime checkIn, DateTime checkOut, int guests)
    {
        var roomIds = await GetAvailableRoomIdsAsync("sp_GetAvailableRoomsByDatesAndGuests",
            new SqlParameter("@CheckIn", checkIn.Date),
            new SqlParameter("@CheckOut", checkOut.Date),
            new SqlParameter("@Guests", guests));

        if (roomIds.Count > 0)
        {
            return await GetRoomsByIdsAsync(roomIds);
        }

        var bookedRoomIds = await _context.Set<ReservationDetail>()
            .Where(rd => rd.Reservation.Status != ReservationStatus.Cancelled
                && rd.Reservation.CheckIn < checkOut
                && rd.Reservation.CheckOut > checkIn)
            .Select(rd => rd.RoomId)
            .Distinct()
            .ToListAsync();

        return await _dbSet
            .Include(r => r.Accommodation)
                .ThenInclude(a => a.Site)
            .Where(r => r.IsActive
                && r.MaxGuests >= guests
                && !bookedRoomIds.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        return !await _context.Set<ReservationDetail>()
            .AnyAsync(rd => rd.RoomId == roomId
                && rd.Reservation.Status != ReservationStatus.Cancelled
                && rd.Reservation.CheckIn < checkOut
                && rd.Reservation.CheckOut > checkIn);
    }

    public async Task<Room?> GetRoomWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(r => r.Accommodation)
                .ThenInclude(a => a.Site)
            .Include(r => r.Rates)
                .ThenInclude(rt => rt.Season)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    private async Task<List<int>> GetAvailableRoomIdsAsync(string procedureName, params SqlParameter[] parameters)
    {
        var roomIds = new List<int>();
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State == System.Data.ConnectionState.Closed;

        if (shouldClose)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        command.CommandText = procedureName;
        command.CommandType = System.Data.CommandType.StoredProcedure;

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            roomIds.Add(reader.GetInt32(reader.GetOrdinal("Id")));
        }

        if (shouldClose)
        {
            await connection.CloseAsync();
        }

        return roomIds;
    }

    private async Task<IEnumerable<Room>> GetRoomsByIdsAsync(IReadOnlyCollection<int> roomIds)
    {
        var rooms = await _dbSet
            .Include(r => r.Accommodation)
                .ThenInclude(a => a.Site)
            .Where(r => roomIds.Contains(r.Id))
            .ToListAsync();

        return rooms.OrderBy(r => roomIds.ToList().IndexOf(r.Id));
    }
}
