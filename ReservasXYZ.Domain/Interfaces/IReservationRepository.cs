using ReservasXYZ.Domain.Entities;

namespace ReservasXYZ.Domain.Interfaces;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<Reservation?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Reservation>> GetByUserAsync(string userId);
    Task<bool> HasOverlappingReservationAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null);
    Task<TResult> ExecuteInSerializableTransactionAsync<TResult>(Func<Task<TResult>> action);
}
