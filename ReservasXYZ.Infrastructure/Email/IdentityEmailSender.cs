using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity.UI.Services;
using ReservasXYZ.Application.Interfaces;

namespace ReservasXYZ.Infrastructure.Email;

public class IdentityEmailSender : IEmailSender
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;

    public IdentityEmailSender(IEmailService emailService, IEmailTemplateService templateService)
    {
        _emailService = emailService;
        _templateService = templateService;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var match = Regex.Match(htmlMessage, @"href=['""]([^'""]+)['""]", RegexOptions.IgnoreCase);
        var actionUrl = match.Success ? match.Groups[1].Value : null;

        var subjectLower = subject.ToLowerInvariant();

        string label, title, message, ctaText;

        if (subjectLower.Contains("confirm") || subjectLower.Contains("activ"))
        {
            label = "Cuenta nueva";
            title = "Confirma tu dirección de correo";
            message = "Gracias por registrarte. Para activar tu cuenta y comenzar a hacer reservas, haz clic en el botón de abajo. Este enlace es válido por 24 horas.";
            ctaText = "Activar cuenta";
        }
        else if (subjectLower.Contains("password") || subjectLower.Contains("contraseña") || subjectLower.Contains("reset"))
        {
            label = "Seguridad";
            title = "Restablece tu contraseña";
            message = "Recibimos una solicitud para restablecer la contraseña de tu cuenta. Si fuiste tú, haz clic en el botón para continuar. Si no lo solicitaste, ignora este correo.";
            ctaText = "Restablecer contraseña";
        }
        else
        {
            label = "Sistema";
            title = subject;
            message = "Tienes un mensaje del sistema de reservas de Fondo XYZ.";
            ctaText = "Ver";
        }

        var html = _templateService.Build(label, title, message, actionUrl != null ? ctaText : null, actionUrl);
        return _emailService.SendEmailAsync(email, subject, html);
    }
}
