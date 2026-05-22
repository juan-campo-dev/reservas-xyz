using System.ComponentModel.DataAnnotations;
using ReservasXYZ.Domain.Enums;

namespace ReservasXYZ.Application.DTOs;

public class ReservationDto
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
    public ReservationStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ReservationDetailDto> Details { get; set; } = new();
}

public class ReservationDetailDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string AccommodationName { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Nights { get; set; }
    public decimal Subtotal { get; set; }
}

public class CreateReservationDto
{
    [Required(ErrorMessage = "El nombre del huésped es obligatorio.")]
    [Display(Name = "Nombre completo")]
    public string GuestName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    [Display(Name = "Correo electrónico")]
    public string GuestEmail { get; set; } = string.Empty;

    [Display(Name = "Teléfono")]
    public string? GuestPhone { get; set; }

    [Display(Name = "Fecha de entrada")]
    public DateTime CheckIn { get; set; }

    [Display(Name = "Fecha de salida")]
    public DateTime CheckOut { get; set; }

    [Display(Name = "Total de huéspedes")]
    public int TotalGuests { get; set; }

    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    public List<int> RoomIds { get; set; } = new();
}

public class AvailabilitySearchDto
{
    public int? SiteId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Guests { get; set; }
}
