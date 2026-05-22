using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReservasXYZ.Domain.Entities;

namespace ReservasXYZ.Infrastructure.Data.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Accommodation> Accommodations => Set<Accommodation>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Rate> Rates => Set<Rate>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationDetail> ReservationDetails => Set<ReservationDetail>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
