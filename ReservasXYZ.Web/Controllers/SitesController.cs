using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;

namespace ReservasXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SitesController : Controller
{
    private readonly ISiteService _siteService;

    public SitesController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    public async Task<IActionResult> Index()
    {
        var sites = await _siteService.GetAllAsync();
        return View(sites);
    }

    public IActionResult Create()
    {
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_SiteForm", new CreateSiteDto());
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSiteDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            return View(dto);
        }
        await _siteService.CreateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Sitio creado exitosamente" });
        TempData["Success"] = "Sitio creado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var site = await _siteService.GetByIdAsync(id);
        if (site is null) return NotFound();
        var dto = new UpdateSiteDto
        {
            Id = site.Id,
            Name = site.Name,
            Address = site.Address,
            City = site.City,
            Country = site.Country,
            Phone = site.Phone,
            Email = site.Email,
            Description = site.Description,
            IsActive = site.IsActive
        };
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_SiteForm", dto);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSiteDto dto)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            return View(dto);
        }
        await _siteService.UpdateAsync(dto);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Sitio actualizado exitosamente" });
        TempData["Success"] = "Sitio actualizado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _siteService.DeleteAsync(id);
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Sitio eliminado exitosamente" });
        TempData["Success"] = "Sitio eliminado exitosamente";
        return RedirectToAction(nameof(Index));
    }
}
