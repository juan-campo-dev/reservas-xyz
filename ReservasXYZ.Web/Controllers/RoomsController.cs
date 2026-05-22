using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;

namespace ReservasXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
public class RoomsController : Controller
{
    private readonly IRoomService _roomService;
    private readonly IAccommodationService _accommodationService;

    public RoomsController(IRoomService roomService, IAccommodationService accommodationService)
    {
        _roomService = roomService;
        _accommodationService = accommodationService;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _roomService.GetAllAsync();
        return View(rooms);
    }

    public async Task<IActionResult> Create()
    {
        await LoadAccommodations();
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_RoomForm", new CreateRoomDto());
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoomDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            await LoadAccommodations();
            return View(dto);
        }
        await _roomService.CreateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Habitación creada exitosamente" });
        TempData["Success"] = "Habitación creada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room is null) return NotFound();
        await LoadAccommodations();
        var dto = new UpdateRoomDto
        {
            Id = room.Id,
            AccommodationId = room.AccommodationId,
            RoomNumber = room.RoomNumber,
            Type = room.Type,
            MaxGuests = room.MaxGuests,
            BasePrice = room.BasePrice,
            Description = room.Description,
            Amenities = room.Amenities,
            IsActive = room.IsActive
        };
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_RoomForm", dto);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRoomDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            await LoadAccommodations();
            return View(dto);
        }
        await _roomService.UpdateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Habitación actualizada exitosamente" });
        TempData["Success"] = "Habitación actualizada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _roomService.DeleteAsync(id);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Habitación eliminada exitosamente" });
        TempData["Success"] = "Habitación eliminada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadAccommodations()
    {
        var accommodations = await _accommodationService.GetAllAsync();
        ViewBag.Accommodations = new SelectList(accommodations, "Id", "Name");
    }
}
