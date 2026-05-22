using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;

namespace ReservasXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AccommodationsController : Controller
{
    private readonly IAccommodationService _accommodationService;
    private readonly ISiteService _siteService;

    public AccommodationsController(IAccommodationService accommodationService, ISiteService siteService)
    {
        _accommodationService = accommodationService;
        _siteService = siteService;
    }

    public async Task<IActionResult> Index()
    {
        var accommodations = await _accommodationService.GetAllAsync();
        return View(accommodations);
    }

    public async Task<IActionResult> Create()
    {
        await LoadSitesAsync();
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_AccommodationForm", new CreateAccommodationDto());
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAccommodationDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            await LoadSitesAsync(dto.SiteId);
            return View(dto);
        }

        await _accommodationService.CreateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Alojamiento creado exitosamente" });
        TempData["Success"] = "Alojamiento creado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var accommodation = await _accommodationService.GetByIdAsync(id);
        if (accommodation is null) return NotFound();

        await LoadSitesAsync(accommodation.SiteId);
        var dto = new UpdateAccommodationDto
        {
            Id = accommodation.Id,
            SiteId = accommodation.SiteId,
            Name = accommodation.Name,
            Description = accommodation.Description,
            Type = accommodation.Type,
            TotalRooms = accommodation.TotalRooms,
            IsActive = accommodation.IsActive
        };
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_AccommodationForm", dto);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateAccommodationDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            await LoadSitesAsync(dto.SiteId);
            return View(dto);
        }

        await _accommodationService.UpdateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Alojamiento actualizado exitosamente" });
        TempData["Success"] = "Alojamiento actualizado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _accommodationService.DeleteAsync(id);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Alojamiento eliminado exitosamente" });
        TempData["Success"] = "Alojamiento eliminado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSitesAsync(int? selectedSiteId = null)
    {
        var sites = await _siteService.GetAllAsync();
        ViewBag.Sites = new SelectList(sites, "Id", "Name", selectedSiteId);
    }
}
