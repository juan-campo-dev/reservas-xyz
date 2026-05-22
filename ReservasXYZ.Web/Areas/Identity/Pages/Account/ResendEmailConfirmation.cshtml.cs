using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ReservasXYZ.Domain.Entities;

namespace ReservasXYZ.Web.Areas.Identity.Pages.Account;

public class ResendEmailConfirmationModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ResendEmailConfirmationModel> _logger;

    public ResendEmailConfirmationModel(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<ResendEmailConfirmationModel> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? StatusMessage { get; private set; }

    public class InputModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet(string? email = null)
    {
        Input = new InputModel
        {
            Email = email ?? string.Empty
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user is not null && !await _userManager.IsEmailConfirmedAsync(user))
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code },
                protocol: Request.Scheme);

            if (!string.IsNullOrWhiteSpace(callbackUrl))
            {
                try
                {
                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Confirma tu cuenta - Reservas XYZ",
                        $"<h2>Confirma tu cuenta</h2><p>Para activar tu usuario, haz clic <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aquí</a>.</p>");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Email confirmation resend failed for {Email}", Input.Email);
                }
            }
        }

        StatusMessage = "Si el correo corresponde a una cuenta pendiente, reenviamos el enlace de confirmación.";

        return Page();
    }
}