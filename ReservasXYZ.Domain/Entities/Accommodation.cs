namespace ReservasXYZ.Domain.Entities;

public class Accommodation
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty; // Hotel, Hostal, Cabaña, etc.
    public int TotalRooms { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Site Site { get; set; } = null!;
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
