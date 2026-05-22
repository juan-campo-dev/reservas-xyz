using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ReservasXYZ.Domain.Entities;

namespace ReservasXYZ.Web.Areas.Identity.Pages.Account;

public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = "No fue posible confirmar el correo.";
    public string ReturnUrl { get; private set; } = "/";
    public string? Email { get; private set; }

    public async Task<IActionResult> OnGetAsync(string? userId, string? code, string? returnUrl = null)
    {
        ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? Url.Content("~/")! : returnUrl;

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
        {
            Message = "El enlace de confirmación no es válido.";
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            Message = "No encontramos la cuenta asociada a este enlace.";
            return Page();
        }

        Email = user.Email;

        string decodedCode;
        try
        {
            decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            Message = "El enlace de confirmación no es válido o ya expiró.";
            return Page();
        }

        var result = await _userManager.ConfirmEmailAsync(user, decodedCode);
        if (result.Succeeded)
        {
            IsSuccess = true;
            Message = "Tu correo fue confirmado. Ya puedes iniciar sesión.";
            return Page();
        }

        Message = string.Join(" ", result.Errors.Select(error => error.Description));
        if (string.IsNullOrWhiteSpace(Message))
        {
            Message = "No fue posible confirmar el correo.";
        }

        return Page();
    }
}