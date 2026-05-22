using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllAsync();
    Task<IEnumerable<RoomDto>> GetByAccommodationAsync(int accommodationId);
    Task<RoomDto?> GetByIdAsync(int id);
    Task<RoomDto> CreateAsync(CreateRoomDto dto);
    Task UpdateAsync(UpdateRoomDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<RoomSearchResultDto>> SearchAvailableAsync(AvailabilitySearchDto search);
}
