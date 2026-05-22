using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Interfaces;

public interface IRateService
{
    Task<IEnumerable<RateDto>> GetAllAsync();
    Task<IEnumerable<RateDto>> GetByRoomAsync(int roomId);
    Task<RateDto?> GetByIdAsync(int id);
    Task<RateDto> CreateAsync(CreateRateDto dto);
    Task UpdateAsync(UpdateRateDto dto);
    Task DeleteAsync(int id);
    Task<decimal> CalculateTotalAsync(int roomId, DateTime checkIn, DateTime checkOut, int totalGuests = 1);
}
