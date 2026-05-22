namespace ReservasXYZ.Application.DTOs;

public class SiteWithAccommodationsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int TotalCapacity { get; set; }
    public List<AccommodationSummaryDto> Accommodations { get; set; } = new();
}

public class AccommodationSummaryDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TotalRooms { get; set; }
    public string? Description { get; set; }
}
