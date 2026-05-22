using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Interfaces;

public interface IGuestPortalService
{
    Task<GuestPortalDto> GetPortalDataAsync(string userId, string firstName);
}
