using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;

namespace ReservasXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SeasonsController : Controller
{
    private readonly ISeasonService _seasonService;

    public SeasonsController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    public async Task<IActionResult> Index()
    {
        var seasons = await _seasonService.GetAllAsync();
        return View(seasons);
    }

    public IActionResult Create()
    {
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_SeasonForm", new CreateSeasonDto { StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(3), PriceMultiplier = 1.0m });
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSeasonDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            return View(dto);
        }

        await _seasonService.CreateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Temporada creada exitosamente" });
        TempData["Success"] = "Temporada creada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var season = await _seasonService.GetByIdAsync(id);
        if (season is null) return NotFound();

        var dto = new UpdateSeasonDto
        {
            Id = season.Id,
            Name = season.Name,
            StartDate = season.StartDate,
            EndDate = season.EndDate,
            PriceMultiplier = season.PriceMultiplier,
            IsActive = season.IsActive
        };
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_SeasonForm", dto);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSeasonDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            return View(dto);
        }

        await _seasonService.UpdateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Temporada actualizada exitosamente" });
        TempData["Success"] = "Temporada actualizada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _seasonService.DeleteAsync(id);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Temporada eliminada exitosamente" });
        TempData["Success"] = "Temporada eliminada exitosamente";
        return RedirectToAction(nameof(Index));
    }
}
