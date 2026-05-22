using ReservasXYZ.Domain.Enums;

namespace ReservasXYZ.Domain.Entities;

public class Rate
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int SeasonId { get; set; }
    public decimal PricePerNight { get; set; }
    public int BaseGuests { get; set; } = 1;
    public decimal ExtraPersonPrice { get; set; }
    public RateKind Kind { get; set; } = RateKind.Standard;
    public bool IsActive { get; set; } = true;

    public Room Room { get; set; } = null!;
    public Season Season { get; set; } = null!;
}
