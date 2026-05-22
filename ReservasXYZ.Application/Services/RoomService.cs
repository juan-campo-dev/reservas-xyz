using AutoMapper;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Interfaces;

namespace ReservasXYZ.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRateRepository _rateRepository;
    private readonly IMapper _mapper;

    public RoomService(IRoomRepository roomRepository, IRateRepository rateRepository, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _rateRepository = rateRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoomDto>> GetAllAsync()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    public async Task<IEnumerable<RoomDto>> GetByAccommodationAsync(int accommodationId)
    {
        var rooms = await _roomRepository.FindAsync(r => r.AccommodationId == accommodationId);
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    public async Task<RoomDto?> GetByIdAsync(int id)
    {
        var room = await _roomRepository.GetRoomWithDetailsAsync(id);
        return room is null ? null : _mapper.Map<RoomDto>(room);
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
    {
        var room = _mapper.Map<Room>(dto);
        await _roomRepository.AddAsync(room);
        await _roomRepository.SaveChangesAsync();
        return _mapper.Map<RoomDto>(room);
    }

    public async Task UpdateAsync(UpdateRoomDto dto)
    {
        var room = await _roomRepository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"No se encontró la habitación #{dto.Id}.");
        _mapper.Map(dto, room);
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró la habitación #{id}.");
        room.IsActive = false;
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<RoomSearchResultDto>> SearchAvailableAsync(AvailabilitySearchDto search)
    {
        var checkIn = search.CheckIn.Date;
        var checkOut = search.CheckOut.Date <= checkIn ? checkIn.AddDays(1) : search.CheckOut.Date;
        var rooms = await _roomRepository.GetAvailableRoomsAsync(checkIn, checkOut);

        if (search.SiteId.HasValue)
        {
            rooms = rooms.Where(room => room.Accommodation?.SiteId == search.SiteId.Value);
        }

        var results = new List<RoomSearchResultDto>();
        var nights = Math.Max(1, (checkOut - checkIn).Days);

        var requestedGuests = search.Guests <= 0 ? 1 : search.Guests;

        foreach (var room in rooms)
        {
            var guestsForPricing = Math.Max(1, Math.Min(requestedGuests, room.MaxGuests));
            var totalPrice = await _rateRepository.CalculateTotalRateAsync(
                room.Id, checkIn, checkOut, guestsForPricing);

            if (totalPrice <= 0)
            {
                totalPrice = room.BasePrice * nights;
            }

            var pricingRate = (await _rateRepository.GetRatesByRoomAsync(room.Id))
                .Where(r => r.IsActive
                            && r.Season != null
                            && r.Season.IsActive
                            && r.Season.StartDate <= checkIn
                            && r.Season.EndDate >= checkIn
                            && r.Kind == ReservasXYZ.Domain.Enums.RateKind.Standard)
                .OrderByDescending(r => r.Season!.PriceMultiplier)
                .FirstOrDefault();

            results.Add(new RoomSearchResultDto
            {
                RoomId = room.Id,
                SiteId = room.Accommodation?.SiteId ?? 0,
                RoomNumber = room.RoomNumber,
                RoomType = room.Type.ToString(),
                MaxGuests = room.MaxGuests,
                AccommodationName = room.Accommodation?.Name ?? "",
                SiteName = room.Accommodation?.Site?.Name ?? "",
                SiteCity = room.Accommodation?.Site?.City ?? "",
                TotalPrice = totalPrice,
                PricePerNight = totalPrice / nights,
                Nights = nights,
                BaseGuests = pricingRate?.BaseGuests ?? 1,
                ExtraPersonPrice = pricingRate?.ExtraPersonPrice ?? 0m,
                BasePricePerNight = pricingRate?.PricePerNight ?? room.BasePrice,
                GuestsForPricing = guestsForPricing,
                RequestedGuests = requestedGuests
            });
        }

        return results
            .OrderBy(result => result.SiteName)
            .ThenBy(result => result.AccommodationName)
            .ThenBy(result => result.RoomNumber);
    }
}
