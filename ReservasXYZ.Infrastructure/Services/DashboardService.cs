using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Domain.Enums;
using ReservasXYZ.Infrastructure.Data.Context;

namespace ReservasXYZ.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DashboardService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var totalReservations = await _context.Reservations.CountAsync();
        var pending = await _context.Reservations.CountAsync(r => r.Status == ReservationStatus.Pending);
        var confirmed = await _context.Reservations.CountAsync(r => r.Status == ReservationStatus.Confirmed);

        var totalRevenue = await _context.Reservations
            .Where(r => r.Status != ReservationStatus.Cancelled)
            .SumAsync(r => r.TotalAmount);

        var monthlyRevenue = await _context.Reservations
            .Where(r => r.Status != ReservationStatus.Cancelled && r.CreatedAt >= monthStart)
            .SumAsync(r => r.TotalAmount);

        var occupiedToday = await _context.ReservationDetails
            .CountAsync(rd => rd.Reservation.CheckIn <= today
                && rd.Reservation.CheckOut > today
                && rd.Reservation.Status == ReservationStatus.CheckedIn);

        var totalRooms = await _context.Rooms.CountAsync(r => r.IsActive);
        var occupancyRate = totalRooms > 0 ? (double)occupiedToday / totalRooms * 100 : 0;

        var recentReservations = await _context.Reservations
            .Include(r => r.Details).ThenInclude(d => d.Room)
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .ToListAsync();

        return new DashboardDto
        {
            TotalSites = await _context.Sites.CountAsync(s => s.IsActive),
            TotalAccommodations = await _context.Accommodations.CountAsync(a => a.IsActive),
            TotalRooms = totalRooms,
            TotalReservations = totalReservations,
            PendingReservations = pending,
            ConfirmedReservations = confirmed,
            TotalRevenue = totalRevenue,
            MonthlyRevenue = monthlyRevenue,
            OccupiedRoomsToday = occupiedToday,
            OccupancyRate = Math.Round(occupancyRate, 1),
            RecentReservations = _mapper.Map<List<ReservationDto>>(recentReservations)
        };
    }
}
