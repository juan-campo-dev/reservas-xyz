using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Enums;
using ReservasXYZ.Domain.Interfaces;
using ReservasXYZ.Infrastructure.Data.Context;

namespace ReservasXYZ.Infrastructure.Data.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Reservation?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(r => r.Details)
                .ThenInclude(d => d.Room)
                    .ThenInclude(room => room.Accommodation)
                        .ThenInclude(accommodation => accommodation.Site)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reservation>> GetByUserAsync(string userId)
    {
        return await _dbSet
            .Include(r => r.Details)
                .ThenInclude(d => d.Room)
                    .ThenInclude(room => room.Accommodation)
                        .ThenInclude(accommodation => accommodation.Site)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingReservationAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null)
    {
        var storedProcedureResult = await ValidateOverbookingFromStoredProcedureAsync(roomId, checkIn, checkOut, excludeReservationId);
        if (storedProcedureResult.HasValue)
        {
            return storedProcedureResult.Value;
        }

        var query = _context.Set<ReservationDetail>()
            .Where(rd => rd.RoomId == roomId
                && rd.Reservation.Status != ReservationStatus.Cancelled
                && rd.Reservation.CheckIn < checkOut
                && rd.Reservation.CheckOut > checkIn);

        if (excludeReservationId.HasValue)
            query = query.Where(rd => rd.ReservationId != excludeReservationId.Value);

        return await query.AnyAsync();
    }

    private async Task<bool?> ValidateOverbookingFromStoredProcedureAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeReservationId)
    {
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State == System.Data.ConnectionState.Closed;

        if (shouldClose)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "sp_ValidateOverbooking";
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@RoomId", roomId));
        command.Parameters.Add(new SqlParameter("@CheckIn", checkIn.Date));
        command.Parameters.Add(new SqlParameter("@CheckOut", checkOut.Date));
        command.Parameters.Add(new SqlParameter("@ExcludeReservationId", excludeReservationId.HasValue ? excludeReservationId.Value : DBNull.Value));

        var isOverbookedParameter = new SqlParameter("@IsOverbooked", System.Data.SqlDbType.Bit)
        {
            Direction = System.Data.ParameterDirection.Output
        };
        command.Parameters.Add(isOverbookedParameter);

        await command.ExecuteNonQueryAsync();

        if (shouldClose)
        {
            await connection.CloseAsync();
        }

        return isOverbookedParameter.Value is bool isOverbooked ? isOverbooked : null;
    }

    public async Task<TResult> ExecuteInSerializableTransactionAsync<TResult>(Func<Task<TResult>> action)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
