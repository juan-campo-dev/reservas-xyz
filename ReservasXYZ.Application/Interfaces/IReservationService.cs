using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Interfaces;

public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllAsync();
    Task<IEnumerable<ReservationDto>> GetByUserAsync(string userId);
    Task<ReservationDto?> GetByIdAsync(int id);
    Task<ReservationDto> CreateAsync(CreateReservationDto dto, string userId);
    Task ConfirmAsync(int id);
    Task CancelAsync(int id);
    Task CheckInAsync(int id);
    Task CheckOutAsync(int id);
}
