using ReservasXYZ.Domain.Entities;

namespace ReservasXYZ.Domain.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    Task<IEnumerable<Room>> GetAvailableRoomsByGuestsAsync(DateTime checkIn, DateTime checkOut, int guests);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
    Task<Room?> GetRoomWithDetailsAsync(int id);
}
