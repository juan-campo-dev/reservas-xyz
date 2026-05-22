using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
}
