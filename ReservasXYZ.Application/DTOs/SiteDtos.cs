using System.ComponentModel.DataAnnotations;

namespace ReservasXYZ.Application.DTOs;

public class SiteDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int AccommodationCount { get; set; }
}

public class CreateSiteDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [Display(Name = "Dirección")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es obligatoria.")]
    [Display(Name = "Ciudad")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "El país es obligatorio.")]
    [Display(Name = "País")]
    public string Country { get; set; } = string.Empty;

    [Display(Name = "Teléfono")]
    public string? Phone { get; set; }

    [Display(Name = "Correo electrónico")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public string? Email { get; set; }

    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Display(Name = "URL de imagen")]
    public string? ImageUrl { get; set; }
}

public class UpdateSiteDto : CreateSiteDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
