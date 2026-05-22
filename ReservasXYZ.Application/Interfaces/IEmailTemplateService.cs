namespace ReservasXYZ.Application.Interfaces;

public interface IEmailTemplateService
{
    string Build(string label, string title, string message, string? ctaText = null, string? ctaUrl = null);
}
