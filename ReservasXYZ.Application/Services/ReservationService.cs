using AutoMapper;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Enums;
using ReservasXYZ.Domain.Interfaces;

namespace ReservasXYZ.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IRateRepository _rateRepository;
    private readonly IMapper _mapper;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        IRateRepository rateRepository,
        IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _rateRepository = rateRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllAsync()
    {
        var reservations = await _reservationRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<IEnumerable<ReservationDto>> GetByUserAsync(string userId)
    {
        var reservations = await _reservationRepository.GetByUserAsync(userId);
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<ReservationDto?> GetByIdAsync(int id)
    {
        var reservation = await _reservationRepository.GetWithDetailsAsync(id);
        return reservation is null ? null : _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationDto dto, string userId)
    {
        int nights = (dto.CheckOut - dto.CheckIn).Days;
        if (nights <= 0)
            throw new InvalidOperationException("La fecha de check-out debe ser posterior al check-in.");

        var selectedRoomIds = dto.RoomIds.Distinct().ToList();

        if (!selectedRoomIds.Any())
            throw new InvalidOperationException("Selecciona al menos una habitación para reservar.");

        var totalCapacity = 0;

        foreach (var roomId in selectedRoomIds)
        {
            var room = await _roomRepository.GetByIdAsync(roomId)
                ?? throw new InvalidOperationException("Una de las habitaciones seleccionadas ya no existe.");

            totalCapacity += room.MaxGuests;
        }

        if (dto.TotalGuests > totalCapacity)
            throw new InvalidOperationException($"La selección actual cubre hasta {totalCapacity} huéspedes. Agrega más habitaciones o reduce la cantidad de huéspedes.");

        var reservation = await _reservationRepository.ExecuteInSerializableTransactionAsync(async () =>
        {
            var details = new List<ReservationDetail>();
            decimal totalAmount = 0;
            var remainingGuests = dto.TotalGuests;

            foreach (var roomId in selectedRoomIds)
            {
                bool hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(
                    roomId, dto.CheckIn, dto.CheckOut);

                if (hasOverlap)
                    throw new InvalidOperationException("Una de las habitaciones seleccionadas ya no está disponible para esas fechas.");

                var room = await _roomRepository.GetByIdAsync(roomId)
                    ?? throw new InvalidOperationException("Una de las habitaciones seleccionadas ya no existe.");

                var guestsForRoom = Math.Min(remainingGuests, room.MaxGuests);
                if (guestsForRoom < 1) guestsForRoom = 1;
                remainingGuests -= guestsForRoom;
                if (remainingGuests < 0) remainingGuests = 0;

                var totalPrice = await _rateRepository.CalculateTotalRateAsync(
                    roomId, dto.CheckIn, dto.CheckOut, guestsForRoom);

                details.Add(new ReservationDetail
                {
                    RoomId = roomId,
                    PricePerNight = nights > 0 ? totalPrice / nights : 0,
                    Nights = nights,
                    Subtotal = totalPrice
                });

                totalAmount += totalPrice;
            }

            var entity = new Reservation
            {
                UserId = userId,
                GuestName = dto.GuestName,
                GuestEmail = dto.GuestEmail,
                GuestPhone = dto.GuestPhone,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                TotalGuests = dto.TotalGuests,
                TotalAmount = totalAmount,
                Status = ReservationStatus.Pending,
                Notes = dto.Notes,
                Details = details
            };

            await _reservationRepository.AddAsync(entity);
            await _reservationRepository.SaveChangesAsync();
            return entity;
        });

        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task ConfirmAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró la reserva #{id}.");
        reservation.Status = ReservationStatus.Confirmed;
        reservation.UpdatedAt = DateTime.UtcNow;
        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync();
    }

    public async Task CancelAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró la reserva #{id}.");
        reservation.Status = ReservationStatus.Cancelled;
        reservation.UpdatedAt = DateTime.UtcNow;
        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync();
    }

    public async Task CheckInAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró la reserva #{id}.");
        reservation.Status = ReservationStatus.CheckedIn;
        reservation.UpdatedAt = DateTime.UtcNow;
        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync();
    }

    public async Task CheckOutAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró la reserva #{id}.");
        reservation.Status = ReservationStatus.CheckedOut;
        reservation.UpdatedAt = DateTime.UtcNow;
        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync();
    }
}
