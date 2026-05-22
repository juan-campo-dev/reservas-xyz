using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;
using ReservasXYZ.Infrastructure.DependencyInjection;
using ReservasXYZ.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("es-CO");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Este campo es obligatorio.");
    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(f => $"El campo {f} es obligatorio.");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((v, f) => $"El valor '{v}' no es válido para {f}.");
    options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(f => $"El valor proporcionado no es válido para {f}.");
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(v => $"El valor '{v}' no es válido.");
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(f => $"El campo {f} debe ser un número.");
    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Este campo es obligatorio.");
});
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    try
    {
        await DataSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(cultureInfo),
    SupportedCultures = new[] { cultureInfo },
    SupportedUICultures = new[] { cultureInfo }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
