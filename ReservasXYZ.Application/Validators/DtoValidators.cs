using FluentValidation;
using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Validators;

public class CreateSiteDtoValidator : AbstractValidator<CreateSiteDto>
{
    public CreateSiteDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es obligatorio.").MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().WithMessage("La dirección es obligatoria.").MaximumLength(500);
        RuleFor(x => x.City).NotEmpty().WithMessage("La ciudad es obligatoria.").MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().WithMessage("El país es obligatorio.").MaximumLength(100);
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().WithMessage("Ingrese un correo electrónico válido.").When(x => !string.IsNullOrWhiteSpace(x.Email)).MaximumLength(200);
    }
}

public class UpdateSiteDtoValidator : AbstractValidator<UpdateSiteDto>
{
    public UpdateSiteDtoValidator()
    {
        Include(new CreateSiteDtoValidator());
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class CreateAccommodationDtoValidator : AbstractValidator<CreateAccommodationDto>
{
    public CreateAccommodationDtoValidator()
    {
        RuleFor(x => x.SiteId).GreaterThan(0).WithMessage("Seleccione una sede.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es obligatorio.").MaximumLength(200);
        RuleFor(x => x.Type).NotEmpty().WithMessage("El tipo es obligatorio.").MaximumLength(50);
        RuleFor(x => x.TotalRooms).GreaterThan(0).WithMessage("El total de habitaciones debe ser mayor a 0.");
    }
}

public class UpdateAccommodationDtoValidator : AbstractValidator<UpdateAccommodationDto>
{
    public UpdateAccommodationDtoValidator()
    {
        Include(new CreateAccommodationDtoValidator());
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomDtoValidator()
    {
        RuleFor(x => x.AccommodationId).GreaterThan(0).WithMessage("Seleccione un alojamiento.");
        RuleFor(x => x.RoomNumber).NotEmpty().WithMessage("El número de habitación es obligatorio.").MaximumLength(20);
        RuleFor(x => x.MaxGuests).GreaterThan(0).WithMessage("La capacidad máxima debe ser mayor a 0.");
        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).WithMessage("El precio base no puede ser negativo.");
    }
}

public class UpdateRoomDtoValidator : AbstractValidator<UpdateRoomDto>
{
    public UpdateRoomDtoValidator()
    {
        Include(new CreateRoomDtoValidator());
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class CreateSeasonDtoValidator : AbstractValidator<CreateSeasonDto>
{
    public CreateSeasonDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es obligatorio.").MaximumLength(100);
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("La fecha de inicio es obligatoria.");
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).WithMessage("La fecha de fin debe ser igual o posterior a la fecha de inicio.");
        RuleFor(x => x.PriceMultiplier).GreaterThan(0).WithMessage("El multiplicador debe ser mayor a 0.");
    }
}

public class UpdateSeasonDtoValidator : AbstractValidator<UpdateSeasonDto>
{
    public UpdateSeasonDtoValidator()
    {
        Include(new CreateSeasonDtoValidator());
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class CreateRateDtoValidator : AbstractValidator<CreateRateDto>
{
    public CreateRateDtoValidator()
    {
        RuleFor(x => x.RoomId).GreaterThan(0).WithMessage("Seleccione una habitación.");
        RuleFor(x => x.SeasonId).GreaterThan(0).WithMessage("Seleccione una temporada.");
        RuleFor(x => x.PricePerNight).GreaterThanOrEqualTo(0).WithMessage("El precio por noche no puede ser negativo.");
    }
}

public class UpdateRateDtoValidator : AbstractValidator<UpdateRateDto>
{
    public UpdateRateDtoValidator()
    {
        Include(new CreateRateDtoValidator());
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationDtoValidator()
    {
        RuleFor(x => x.GuestName).NotEmpty().WithMessage("El nombre del huésped es obligatorio.").MaximumLength(200);
        RuleFor(x => x.GuestEmail).NotEmpty().WithMessage("El correo electrónico es obligatorio.").EmailAddress().WithMessage("Ingrese un correo electrónico válido.").MaximumLength(200);
        RuleFor(x => x.GuestPhone).MaximumLength(20);
        RuleFor(x => x.CheckIn).NotEmpty().WithMessage("La fecha de check-in es obligatoria.");
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn).WithMessage("La fecha de check-out debe ser posterior al check-in.");
        RuleFor(x => x.TotalGuests).GreaterThan(0).WithMessage("Seleccione al menos un huésped.");
        RuleFor(x => x.RoomIds).NotEmpty().WithMessage("Seleccione al menos una habitación.");
    }
}

public class AvailabilitySearchDtoValidator : AbstractValidator<AvailabilitySearchDto>
{
    public AvailabilitySearchDtoValidator()
    {
        RuleFor(x => x.SiteId).GreaterThan(0).WithMessage("Seleccione una sede válida.").When(x => x.SiteId.HasValue);
        RuleFor(x => x.CheckIn).NotEmpty().WithMessage("La fecha de entrada es obligatoria.");
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn).WithMessage("La fecha de salida debe ser posterior a la de entrada.");
        RuleFor(x => x.Guests).GreaterThan(0).WithMessage("Seleccione al menos un huésped.");
    }
}
