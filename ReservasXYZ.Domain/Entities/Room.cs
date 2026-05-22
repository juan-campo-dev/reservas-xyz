using ReservasXYZ.Domain.Enums;

namespace ReservasXYZ.Domain.Entities;

public class Room
{
    public int Id { get; set; }
    public int AccommodationId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public int MaxGuests { get; set; }
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public string? Amenities { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Accommodation Accommodation { get; set; } = null!;
    public ICollection<Rate> Rates { get; set; } = new List<Rate>();
    public ICollection<ReservationDetail> ReservationDetails { get; set; } = new List<ReservationDetail>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
