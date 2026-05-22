using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Infrastructure.Data.Context;

namespace ReservasXYZ.Infrastructure.Services;

public class GuestPortalService : IGuestPortalService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GuestPortalService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GuestPortalDto> GetPortalDataAsync(string userId, string firstName)
    {
        var reservations = await _context.Reservations
            .Include(r => r.Details).ThenInclude(d => d.Room).ThenInclude(r => r.Accommodation).ThenInclude(a => a.Site)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CheckIn)
            .Take(10)
            .ToListAsync();

        var sites = await _context.Sites
            .Include(s => s.Accommodations).ThenInclude(a => a.Rooms)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return new GuestPortalDto
        {
            FirstName = firstName,
            MyReservations = _mapper.Map<List<ReservationDto>>(reservations),
            Sites = sites.Select(s => new SiteWithAccommodationsDto
            {
                Id = s.Id,
                Name = s.Name,
                City = s.City,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                TotalCapacity = s.Accommodations
                    .Where(a => a.IsActive)
                    .SelectMany(a => a.Rooms)
                    .Where(r => r.IsActive)
                    .Sum(r => r.MaxGuests),
                Accommodations = s.Accommodations
                    .Where(a => a.IsActive)
                    .Select(a => new AccommodationSummaryDto
                    {
                        Name = a.Name,
                        Type = a.Type,
                        TotalRooms = a.TotalRooms,
                        Description = a.Description
                    }).ToList()
            }).ToList()
        };
    }
}
