using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;

namespace ReservasXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
public class RatesController : Controller
{
    private readonly IRateService _rateService;
    private readonly IRoomService _roomService;
    private readonly ISeasonService _seasonService;

    public RatesController(IRateService rateService, IRoomService roomService, ISeasonService seasonService)
    {
        _rateService = rateService;
        _roomService = roomService;
        _seasonService = seasonService;
    }

    public async Task<IActionResult> Index()
    {
        var rates = await _rateService.GetAllAsync();
        return View(rates);
    }

    public async Task<IActionResult> Create()
    {
        await LoadOptionsAsync();
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_RateForm", new CreateRateDto());
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRateDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            await LoadOptionsAsync(dto.RoomId, dto.SeasonId);
            return View(dto);
        }

        await _rateService.CreateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Tarifa creada exitosamente" });
        TempData["Success"] = "Tarifa creada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var rate = await _rateService.GetByIdAsync(id);
        if (rate is null) return NotFound();

        await LoadOptionsAsync(rate.RoomId, rate.SeasonId);
        var dto = new UpdateRateDto
        {
            Id = rate.Id,
            RoomId = rate.RoomId,
            SeasonId = rate.SeasonId,
            PricePerNight = rate.PricePerNight,
            IsActive = rate.IsActive
        };
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_RateForm", dto);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRateDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            await LoadOptionsAsync(dto.RoomId, dto.SeasonId);
            return View(dto);
        }

        await _rateService.UpdateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Tarifa actualizada exitosamente" });
        TempData["Success"] = "Tarifa actualizada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _rateService.DeleteAsync(id);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Tarifa eliminada exitosamente" });
        TempData["Success"] = "Tarifa eliminada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadOptionsAsync(int? selectedRoomId = null, int? selectedSeasonId = null)
    {
        var rooms = await _roomService.GetAllAsync();
        var seasons = await _seasonService.GetAllAsync();

        ViewBag.Rooms = rooms.Select(room => new SelectListItem
        {
            Value = room.Id.ToString(),
            Text = $"{room.SiteName} - {room.AccommodationName} - {room.RoomNumber}",
            Selected = selectedRoomId == room.Id
        }).ToList();

        ViewBag.Seasons = new SelectList(seasons, "Id", "Name", selectedSeasonId);
    }
}
