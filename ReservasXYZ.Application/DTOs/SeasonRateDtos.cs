using System.ComponentModel.DataAnnotations;

namespace ReservasXYZ.Application.DTOs;

public class SeasonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PriceMultiplier { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSeasonDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [Display(Name = "Fecha de inicio")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    [Display(Name = "Fecha de fin")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "El multiplicador de precio es obligatorio.")]
    [Display(Name = "Multiplicador de precio")]
    public decimal PriceMultiplier { get; set; } = 1.0m;
}

public class UpdateSeasonDto : CreateSeasonDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}

public class RateDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int BaseGuests { get; set; } = 1;
    public decimal ExtraPersonPrice { get; set; }
    public ReservasXYZ.Domain.Enums.RateKind Kind { get; set; }
    public bool IsActive { get; set; }
}

public class CreateRateDto
{
    [Required(ErrorMessage = "La habitación es obligatoria.")]
    [Display(Name = "Habitación")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "La temporada es obligatoria.")]
    [Display(Name = "Temporada")]
    public int SeasonId { get; set; }

    [Required(ErrorMessage = "El precio por noche es obligatorio.")]
    [Display(Name = "Precio por noche")]
    public decimal PricePerNight { get; set; }

    [Display(Name = "Huéspedes base")]
    public int BaseGuests { get; set; } = 1;

    [Display(Name = "Precio por persona extra")]
    public decimal ExtraPersonPrice { get; set; }

    [Display(Name = "Tipo de tarifa")]
    public ReservasXYZ.Domain.Enums.RateKind Kind { get; set; } = ReservasXYZ.Domain.Enums.RateKind.Standard;
}

public class UpdateRateDto : CreateRateDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
