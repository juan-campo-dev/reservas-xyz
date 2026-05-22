using System.ComponentModel.DataAnnotations;

namespace ReservasXYZ.Application.DTOs;

public class AccommodationDto
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public int TotalRooms { get; set; }
    public bool IsActive { get; set; }
}

public class CreateAccommodationDto
{
    [Required(ErrorMessage = "La sede es obligatoria.")]
    [Display(Name = "Sede")]
    public int SiteId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [Display(Name = "Tipo")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "El total de habitaciones es obligatorio.")]
    [Display(Name = "Total de habitaciones")]
    public int TotalRooms { get; set; }
}

public class UpdateAccommodationDto : CreateAccommodationDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
