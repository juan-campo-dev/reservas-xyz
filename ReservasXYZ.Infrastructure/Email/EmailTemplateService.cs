using ReservasXYZ.Application.Interfaces;

namespace ReservasXYZ.Infrastructure.Email;

public class EmailTemplateService : IEmailTemplateService
{
    private const string PrimaryColor = "#DC2626";
    private const string BrandName = "RESERVAS XYZ";
    private const string FooterName = "Fondo XYZ &middot; Sistema de reservas";

    public string Build(string label, string title, string message, string? ctaText = null, string? ctaUrl = null)
    {
        var ctaBlock = ctaText != null && ctaUrl != null
            ? $@"
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin:32px 0 0;"">
                <tr>
                  <td align=""center"">
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""background:{PrimaryColor};border-radius:999px;text-align:center;"">
                          <a href=""{ctaUrl}"" style=""color:#ffffff;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;font-size:15px;font-weight:600;text-decoration:none;display:block;padding:16px 24px;"">{ctaText}</a>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
              <p style=""margin:20px 0 0;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;font-size:12px;color:#9CA3AF;text-align:center;"">
                Si el botón no funciona, copia este enlace:<br>
                <a href=""{ctaUrl}"" style=""color:{PrimaryColor};word-break:break-all;font-size:11px;"">{ctaUrl}</a>
              </p>"
            : string.Empty;

        return $@"<!DOCTYPE html>
<html lang=""es"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width,initial-scale=1.0"">
  <title>{title}</title>
</head>
<body style=""margin:0;padding:0;background:#F3F4F6;-webkit-font-smoothing:antialiased;"">
  <span style=""display:none;max-height:0;overflow:hidden;mso-hide:all;"">{message}</span>
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#F3F4F6;padding:48px 16px;"">
    <tr>
      <td align=""center"">
        <table width=""520"" cellpadding=""0"" cellspacing=""0"" style=""max-width:520px;width:100%;"">

          <!-- Header -->
          <tr>
            <td style=""background:{PrimaryColor};border-radius:16px 16px 0 0;padding:24px 36px;"">
              <span style=""font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;font-size:18px;font-weight:700;color:#ffffff;letter-spacing:0.5px;"">{BrandName}</span>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style=""background:#ffffff;padding:36px 36px 32px;"">
              <p style=""margin:0 0 8px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;font-size:11px;font-weight:600;color:#9CA3AF;text-transform:uppercase;letter-spacing:1.5px;"">{label}</p>
              <h1 style=""margin:0 0 16px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;font-size:24px;font-weight:700;color:#111827;line-height:1.3;"">{title}</h1>
              <p style=""margin:0;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;font-size:15px;color:#6B7280;line-height:1.7;"">{message}</p>
              {ctaBlock}
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""background:#F9FAFB;border-top:1px solid #E5E7EB;border-radius:0 0 16px 16px;padding:20px 36px;text-align:center;"">
              <p style=""margin:0;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;font-size:12px;color:#9CA3AF;"">{FooterName}</p>
              <p style=""margin:6px 0 0;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;font-size:11px;color:#D1D5DB;"">Si no solicitaste esta acción, ignora este correo.</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
    }
}
