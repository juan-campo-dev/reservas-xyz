using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Interfaces;

public interface ISiteService
{
    Task<IEnumerable<SiteDto>> GetAllAsync();
    Task<SiteDto?> GetByIdAsync(int id);
    Task<SiteDto> CreateAsync(CreateSiteDto dto);
    Task UpdateAsync(UpdateSiteDto dto);
    Task DeleteAsync(int id);
}
