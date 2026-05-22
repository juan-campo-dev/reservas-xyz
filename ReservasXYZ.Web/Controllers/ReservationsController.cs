using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;

namespace ReservasXYZ.Web.Controllers;

[Authorize]
public class ReservationsController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly IRoomService _roomService;
    private readonly IRateService _rateService;
    private readonly IEmailService _emailService;

    public ReservationsController(
        IReservationService reservationService,
        IRoomService roomService,
        IRateService rateService,
        IEmailService emailService)
    {
        _reservationService = reservationService;
        _roomService = roomService;
        _rateService = rateService;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var reservations = isAdmin
            ? await _reservationService.GetAllAsync()
            : await _reservationService.GetByUserAsync(userId);

        return View(reservations);
    }

    public async Task<IActionResult> Details(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation is null) return NotFound();

        if (!User.IsInRole("Admin") && reservation.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid();
        }

        return View(reservation);
    }

    public async Task<IActionResult> Create(int? roomId, [FromQuery] List<int>? roomIds, DateTime checkIn, DateTime checkOut, int guests)
    {
        var selectedRoomIds = new List<int>();

        if (roomId.HasValue)
        {
            selectedRoomIds.Add(roomId.Value);
        }

        if (roomIds is not null)
        {
            selectedRoomIds.AddRange(roomIds);
        }

        selectedRoomIds = selectedRoomIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (!selectedRoomIds.Any()) return NotFound();

        var rooms = await GetRoomsAsync(selectedRoomIds);
        if (!rooms.Any()) return NotFound();

        var effectiveCheckIn = checkIn.Date;
        var effectiveCheckOut = checkOut.Date <= effectiveCheckIn ? effectiveCheckIn.AddDays(1) : checkOut.Date;
        var effectiveGuests = Math.Max(1, guests);

        await LoadReservationSummaryAsync(rooms, effectiveCheckIn, effectiveCheckOut, effectiveGuests);

        var model = new CreateReservationDto
        {
            CheckIn = effectiveCheckIn,
            CheckOut = effectiveCheckOut,
            TotalGuests = effectiveGuests,
            RoomIds = rooms.Select(room => room.Id).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReservationDto dto)
    {
        if (dto.CheckOut <= dto.CheckIn)
        {
            ModelState.AddModelError(string.Empty, "La fecha de check-out debe ser posterior al check-in.");
        }

        if (!dto.RoomIds.Any())
        {
            ModelState.AddModelError(string.Empty, "Selecciona al menos una habitación.");
        }

        if (dto.TotalGuests < 1)
        {
            ModelState.AddModelError(string.Empty, "Selecciona al menos un huésped.");
        }

        if (!ModelState.IsValid)
        {
            await LoadCreateViewDataAsync(dto);

            return View(dto);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        ReservationDto reservation;

        try
        {
            reservation = await _reservationService.CreateAsync(dto, userId);
            reservation = await _reservationService.GetByIdAsync(reservation.Id) ?? reservation;
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadCreateViewDataAsync(dto);

            return View(dto);
        }

        try
        {
            await _emailService.SendEmailAsync(
                dto.GuestEmail,
                "Confirmación de reserva - Reservas XYZ",
                $"<h2>Reserva #{reservation.Id}</h2><p>Su reserva ha sido registrada. Check-in: {dto.CheckIn:dd/MM/yyyy}, Check-out: {dto.CheckOut:dd/MM/yyyy}. Total: ${reservation.TotalAmount:N0}</p>");
        }
        catch { }

        TempData["Success"] = "Reserva creada exitosamente";
        return RedirectToAction(nameof(Details), new { id = reservation.Id });
    }

    private async Task LoadCreateViewDataAsync(CreateReservationDto dto)
    {
        var rooms = await GetRoomsAsync(dto.RoomIds);

        if (rooms.Any())
        {
            await LoadReservationSummaryAsync(rooms, dto.CheckIn.Date, dto.CheckOut.Date, Math.Max(1, dto.TotalGuests));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int id)
    {
        await _reservationService.ConfirmAsync(id);

        TempData["Success"] = "Reserva confirmada";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation is null) return NotFound();

        if (!User.IsInRole("Admin"))
        {
            if (reservation.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            if (reservation.Status != ReservasXYZ.Domain.Enums.ReservationStatus.Pending)
            {
                TempData["Error"] = "Solo puedes cancelar reservas pendientes";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        await _reservationService.CancelAsync(id);

        TempData["Success"] = "Reserva cancelada";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(int id)
    {
        await _reservationService.CheckInAsync(id);

        TempData["Success"] = "Check-in realizado";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOut(int id)
    {
        await _reservationService.CheckOutAsync(id);

        TempData["Success"] = "Check-out realizado";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadReservationSummaryAsync(RoomDto room, DateTime checkIn, DateTime checkOut, int guests = 1)
    {
        await LoadReservationSummaryAsync(new[] { room }, checkIn, checkOut, guests);
    }

    private async Task LoadReservationSummaryAsync(IEnumerable<RoomDto> rooms, DateTime checkIn, DateTime checkOut, int guests = 1)
    {
        var selectedRooms = rooms.ToList();
        var effectiveCheckOut = checkOut <= checkIn ? checkIn.AddDays(1) : checkOut;
        var nights = Math.Max(1, (effectiveCheckOut - checkIn).Days);
        var roomTotals = new Dictionary<int, decimal>();
        var roomExtras = new Dictionary<int, (int GuestsAssigned, int BaseGuests, decimal ExtraPersonPrice)>();
        decimal totalPrice = 0;
        var remainingGuests = Math.Max(1, guests);

        foreach (var room in selectedRooms)
        {
            var guestsForRoom = Math.Min(remainingGuests, room.MaxGuests);
            if (guestsForRoom < 1) guestsForRoom = 1;
            remainingGuests -= guestsForRoom;
            if (remainingGuests < 0) remainingGuests = 0;

            var roomTotal = await _rateService.CalculateTotalAsync(room.Id, checkIn, effectiveCheckOut, guestsForRoom);

            if (roomTotal <= 0)
            {
                roomTotal = room.BasePrice * nights;
            }

            roomTotals[room.Id] = roomTotal;
            totalPrice += roomTotal;

            var pricingRate = (await _rateService.GetByRoomAsync(room.Id))
                .Where(r => r.IsActive && r.Kind == ReservasXYZ.Domain.Enums.RateKind.Standard)
                .OrderByDescending(r => r.PricePerNight)
                .FirstOrDefault();

            roomExtras[room.Id] = (
                guestsForRoom,
                pricingRate?.BaseGuests ?? 1,
                pricingRate?.ExtraPersonPrice ?? 0m
            );
        }

        ViewBag.CheckIn = checkIn;
        ViewBag.CheckOut = effectiveCheckOut;
        ViewBag.Nights = nights;
        ViewBag.TotalPrice = totalPrice;
        ViewBag.PricePerNight = totalPrice / nights;
        ViewBag.Room = selectedRooms.FirstOrDefault();
        ViewBag.Rooms = selectedRooms;
        ViewBag.RoomTotals = roomTotals;
        ViewBag.RoomExtras = roomExtras;
    }

    private async Task<List<RoomDto>> GetRoomsAsync(IEnumerable<int> roomIds)
    {
        var rooms = new List<RoomDto>();

        foreach (var roomId in roomIds.Where(id => id > 0).Distinct())
        {
            var room = await _roomService.GetByIdAsync(roomId);

            if (room is not null)
            {
                rooms.Add(room);
            }
        }

        return rooms;
    }

}
