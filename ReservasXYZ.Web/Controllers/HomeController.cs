using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Web.Models;
using ReservasXYZ.Web.ViewModels;

namespace ReservasXYZ.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IGuestPortalService _guestPortalService;
    private readonly IRoomService _roomService;
    private readonly IFavoriteService _favoriteService;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(
        IDashboardService dashboardService,
        IGuestPortalService guestPortalService,
        IRoomService roomService,
        IFavoriteService favoriteService,
        UserManager<ApplicationUser> userManager)
    {
        _dashboardService = dashboardService;
        _guestPortalService = guestPortalService;
        _roomService = roomService;
        _favoriteService = favoriteService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Admin"))
        {
            var dashboard = await _dashboardService.GetDashboardAsync();
            return View(dashboard);
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = _userManager.GetUserId(User)!;
            var user = await _userManager.FindByIdAsync(userId);
            var portal = await _guestPortalService.GetPortalDataAsync(userId, user?.FirstName ?? "Asociado");
            return View(portal);
        }

        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchAvailability(AvailabilitySearchDto search)
    {
        if (User.IsInRole("Admin"))
        {
            return Forbid();
        }

        if (search.CheckOut <= search.CheckIn)
        {
            ModelState.AddModelError(string.Empty, "La fecha de salida debe ser posterior a la fecha de entrada.");
        }

        if (search.Guests < 1)
        {
            ModelState.AddModelError(string.Empty, "Selecciona al menos un huésped.");
        }

        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_GuestAvailabilityResults", new GuestAvailabilityResultsViewModel
            {
                Search = search,
                Rooms = new List<RoomSearchResultDto>()
            });
        }

        search.CheckIn = search.CheckIn.Date;
        search.CheckOut = search.CheckOut.Date;

        var rooms = (await _roomService.SearchAvailableAsync(search)).ToList();
        var userId = _userManager.GetUserId(User)!;
        var favoriteRoomIds = await _favoriteService.GetFavoriteRoomIdsAsync(userId, rooms.Select(room => room.RoomId));
        var user = await _userManager.FindByIdAsync(userId);
        var portal = await _guestPortalService.GetPortalDataAsync(userId, user?.FirstName ?? "Asociado");
        var selectedSiteName = search.SiteId.HasValue
            ? portal.Sites.FirstOrDefault(site => site.Id == search.SiteId.Value)?.Name
            : null;

        return PartialView("_GuestAvailabilityResults", new GuestAvailabilityResultsViewModel
        {
            Search = search,
            Rooms = rooms,
            SelectedSiteName = selectedSiteName,
            FavoriteRoomIds = favoriteRoomIds.ToHashSet()
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
