using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;

namespace ReservasXYZ.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();

        try
        {
            _logger.LogInformation("SMTP connecting to {Server}:{Port} for {To}...", _settings.SmtpServer, _settings.SmtpPort, toEmail);
            var secureOption = _settings.SmtpPort == 465
                ? SecureSocketOptions.SslOnConnect
                : (_settings.SmtpPort == 587 ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, secureOption);
            _logger.LogInformation("SMTP connected. Authenticating...");
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            _logger.LogInformation("SMTP authenticated. Sending...");
            await client.SendAsync(message);
            _logger.LogInformation("SMTP sent OK to {To}, subject: {Subject}", toEmail, subject);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP failed for {To}: {Message}", toEmail, ex.Message);
            throw;
        }
    }
}
