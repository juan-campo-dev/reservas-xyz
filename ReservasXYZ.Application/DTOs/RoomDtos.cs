using System.ComponentModel.DataAnnotations;
using ReservasXYZ.Domain.Enums;

namespace ReservasXYZ.Application.DTOs;

public class RoomDto
{
    public int Id { get; set; }
    public int AccommodationId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public string TypeName => Type.ToString();
    public int MaxGuests { get; set; }
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public string? Amenities { get; set; }
    public string AccommodationName { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class RoomSearchResultDto
{
    public int RoomId { get; set; }
    public int SiteId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public int MaxGuests { get; set; }
    public string AccommodationName { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;
    public string SiteCity { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public decimal PricePerNight { get; set; }
    public int Nights { get; set; }
    public int BaseGuests { get; set; } = 1;
    public decimal ExtraPersonPrice { get; set; }
    public decimal BasePricePerNight { get; set; }
    public int GuestsForPricing { get; set; } = 1;
    public int RequestedGuests { get; set; } = 1;
    public int ExtraGuestsCharged => Math.Max(0, GuestsForPricing - BaseGuests);
}

public class CreateRoomDto
{
    [Required(ErrorMessage = "El alojamiento es obligatorio.")]
    [Display(Name = "Alojamiento")]
    public int AccommodationId { get; set; }

    [Required(ErrorMessage = "El número de habitación es obligatorio.")]
    [Display(Name = "Número de habitación")]
    public string RoomNumber { get; set; } = string.Empty;

    [Display(Name = "Tipo")]
    public RoomType Type { get; set; }

    [Required(ErrorMessage = "La capacidad máxima es obligatoria.")]
    [Display(Name = "Capacidad máxima")]
    public int MaxGuests { get; set; }

    [Required(ErrorMessage = "El precio base es obligatorio.")]
    [Display(Name = "Precio base")]
    public decimal BasePrice { get; set; }

    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Display(Name = "Amenidades")]
    public string? Amenities { get; set; }
}

public class UpdateRoomDto : CreateRoomDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
