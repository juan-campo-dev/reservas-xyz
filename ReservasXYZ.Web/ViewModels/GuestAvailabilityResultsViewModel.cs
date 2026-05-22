using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Web.ViewModels;

public class GuestAvailabilityResultsViewModel
{
    public AvailabilitySearchDto Search { get; set; } = new();
    public List<RoomSearchResultDto> Rooms { get; set; } = new();
    public string? SelectedSiteName { get; set; }
    public HashSet<int> FavoriteRoomIds { get; set; } = new();
}