namespace ReservasXYZ.Application.DTOs;

public class GuestPortalDto
{
    public string FirstName { get; set; } = string.Empty;
    public List<ReservationDto> MyReservations { get; set; } = new();
    public List<SiteWithAccommodationsDto> Sites { get; set; } = new();
}
