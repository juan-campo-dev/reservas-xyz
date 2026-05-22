using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Application.Mappings;
using ReservasXYZ.Application.Services;
using ReservasXYZ.Application.Validators;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Interfaces;
using ReservasXYZ.Infrastructure.Data.Context;
using ReservasXYZ.Infrastructure.Data.Repositories;
using ReservasXYZ.Infrastructure.Email;
using ReservasXYZ.Infrastructure.Identity;
using ReservasXYZ.Infrastructure.Services;

namespace ReservasXYZ.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        var requireConfirmedAccount = configuration.GetValue<bool?>("Identity:RequireConfirmedAccount") ?? true;

        services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = requireConfirmedAccount;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.User.RequireUniqueEmail = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddRoles<IdentityRole>()
        .AddErrorDescriber<SpanishIdentityErrorDescriber>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        ValidatorOptions.Global.LanguageManager = new SpanishFluentValidationLanguageManager();

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEmailTemplateService, EmailTemplateService>();
        services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, IdentityEmailSender>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IRateRepository, RateRepository>();

        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<IAccommodationService, AccommodationService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ISeasonService, SeasonService>();
        services.AddScoped<IRateService, RateService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IDashboardService, Infrastructure.Services.DashboardService>();
        services.AddScoped<IGuestPortalService, Infrastructure.Services.GuestPortalService>();

        services.AddAutoMapper(typeof(MappingProfile));
        services.AddValidatorsFromAssemblyContaining<CreateSiteDtoValidator>();

        return services;
    }
}
