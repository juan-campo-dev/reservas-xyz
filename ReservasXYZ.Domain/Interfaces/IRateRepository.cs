using ReservasXYZ.Domain.Entities;

namespace ReservasXYZ.Domain.Interfaces;

public interface IRateRepository : IRepository<Rate>
{
    Task<decimal> GetRateForRoomAsync(int roomId, DateTime date);
    Task<decimal> CalculateTotalRateAsync(int roomId, DateTime checkIn, DateTime checkOut, int totalGuests = 1);
    Task<IEnumerable<Rate>> GetRatesByRoomAsync(int roomId);
}
