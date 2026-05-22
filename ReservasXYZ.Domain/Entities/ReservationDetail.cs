namespace ReservasXYZ.Domain.Entities;

public class ReservationDetail
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public int RoomId { get; set; }
    public decimal PricePerNight { get; set; }
    public int Nights { get; set; }
    public decimal Subtotal { get; set; }

    public Reservation Reservation { get; set; } = null!;
    public Room Room { get; set; } = null!;
}
