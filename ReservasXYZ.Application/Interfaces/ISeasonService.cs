using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Interfaces;

public interface ISeasonService
{
    Task<IEnumerable<SeasonDto>> GetAllAsync();
    Task<SeasonDto?> GetByIdAsync(int id);
    Task<SeasonDto> CreateAsync(CreateSeasonDto dto);
    Task UpdateAsync(UpdateSeasonDto dto);
    Task DeleteAsync(int id);
}
