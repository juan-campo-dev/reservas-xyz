using ReservasXYZ.Domain.Enums;

namespace ReservasXYZ.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string? GuestPhone { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int TotalGuests { get; set; }
    public decimal TotalAmount { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ICollection<ReservationDetail> Details { get; set; } = new List<ReservationDetail>();
}
