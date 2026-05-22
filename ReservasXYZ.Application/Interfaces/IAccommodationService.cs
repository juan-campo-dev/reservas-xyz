using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Interfaces;

public interface IAccommodationService
{
    Task<IEnumerable<AccommodationDto>> GetAllAsync();
    Task<IEnumerable<AccommodationDto>> GetBySiteAsync(int siteId);
    Task<AccommodationDto?> GetByIdAsync(int id);
    Task<AccommodationDto> CreateAsync(CreateAccommodationDto dto);
    Task UpdateAsync(UpdateAccommodationDto dto);
    Task DeleteAsync(int id);
}
