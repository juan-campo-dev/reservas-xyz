using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Enums;

namespace ReservasXYZ.Infrastructure.Data.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Address).HasMaxLength(500).IsRequired();
        builder.Property(s => s.City).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Country).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.Email).HasMaxLength(200);
        builder.HasIndex(s => s.Name);
    }
}

public class AccommodationConfiguration : IEntityTypeConfiguration<Accommodation>
{
    public void Configure(EntityTypeBuilder<Accommodation> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Type).HasMaxLength(50).IsRequired();
        builder.HasOne(a => a.Site)
            .WithMany(s => s.Accommodations)
            .HasForeignKey(a => a.SiteId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(a => a.SiteId);
    }
}

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RoomNumber).HasMaxLength(20).IsRequired();
        builder.Property(r => r.BasePrice).HasColumnType("decimal(18,2)");
        builder.Property(r => r.Type).HasConversion<int>();
        builder.HasOne(r => r.Accommodation)
            .WithMany(a => a.Rooms)
            .HasForeignKey(r => r.AccommodationId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(r => new { r.AccommodationId, r.RoomNumber }).IsUnique();
    }
}

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.Property(s => s.PriceMultiplier).HasColumnType("decimal(5,2)");
        builder.HasIndex(s => new { s.StartDate, s.EndDate });
    }
}

public class RateConfiguration : IEntityTypeConfiguration<Rate>
{
    public void Configure(EntityTypeBuilder<Rate> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.PricePerNight).HasColumnType("decimal(18,2)");
        builder.Property(r => r.ExtraPersonPrice).HasColumnType("decimal(18,2)");
        builder.Property(r => r.BaseGuests).HasDefaultValue(1);
        builder.Property(r => r.Kind).HasConversion<int>().HasDefaultValue(RateKind.Standard);
        builder.HasOne(r => r.Room)
            .WithMany(rm => rm.Rates)
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.Season)
            .WithMany(s => s.Rates)
            .HasForeignKey(r => r.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(r => new { r.RoomId, r.SeasonId, r.Kind }).IsUnique();
    }
}

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.GuestName).HasMaxLength(200).IsRequired();
        builder.Property(r => r.GuestEmail).HasMaxLength(200).IsRequired();
        builder.Property(r => r.GuestPhone).HasMaxLength(20);
        builder.Property(r => r.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(r => r.Status).HasConversion<int>();
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new { r.CheckIn, r.CheckOut });
    }
}

public class ReservationDetailConfiguration : IEntityTypeConfiguration<ReservationDetail>
{
    public void Configure(EntityTypeBuilder<ReservationDetail> builder)
    {
        builder.HasKey(rd => rd.Id);
        builder.Property(rd => rd.PricePerNight).HasColumnType("decimal(18,2)");
        builder.Property(rd => rd.Subtotal).HasColumnType("decimal(18,2)");
        builder.HasOne(rd => rd.Reservation)
            .WithMany(r => r.Details)
            .HasForeignKey(rd => rd.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(rd => rd.Room)
            .WithMany(r => r.ReservationDetails)
            .HasForeignKey(rd => rd.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(rd => new { rd.RoomId, rd.ReservationId });
    }
}

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.UserId).HasMaxLength(450).IsRequired();
        builder.Property(f => f.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(f => f.Room)
            .WithMany(r => r.Favorites)
            .HasForeignKey(f => f.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(f => new { f.UserId, f.RoomId }).IsUnique();
        builder.HasIndex(f => f.RoomId);
    }
}
