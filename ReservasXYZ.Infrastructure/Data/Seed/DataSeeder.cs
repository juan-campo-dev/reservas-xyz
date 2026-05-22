using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Infrastructure.Data.Context;

namespace ReservasXYZ.Infrastructure.Data.Seed;

public static class DataSeeder
{
    private static readonly string[] Roles = { "Admin", "Receptionist", "Guest", "Cliente" };

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DataSeeder");

        await context.Database.MigrateAsync();
        await ExecuteSqlScriptAsync(context, FindScriptPath("StoredProcedures", "StoredProcedures.sql"), "StoredProcedures.sql", logger);
        await EnsureRolesAsync(roleManager);
        await EnsureAdminAsync(userManager);
        await EnsureTestUsersAsync(userManager, logger);
        await EnsureCatalogAsync(context, logger);
    }

    private static async Task EnsureTestUsersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        await EnsureUserAsync(userManager, logger,
            email: "admin@test.com",
            password: "Admin123*",
            firstName: "Admin",
            lastName: "Test",
            documentNumber: "ADM-TEST",
            role: "Admin");

        await EnsureUserAsync(userManager, logger,
            email: "cliente@test.com",
            password: "Cliente123*",
            firstName: "Cliente",
            lastName: "Test",
            documentNumber: "CLI-TEST",
            role: "Cliente");
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string email,
        string password,
        string firstName,
        string lastName,
        string documentNumber,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                DocumentNumber = documentNumber,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                logger.LogWarning("Test user {Email} could not be created: {Errors}", email, errors);
                return;
            }
        }
        else if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task EnsureAdminAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@hotel.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "Hotel",
                DocumentNumber = "000000000",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Admin user could not be created: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    private static async Task EnsureCatalogAsync(ApplicationDbContext context, ILogger logger)
    {
        var scriptPath = FindScriptPath("database", "SeedCatalog.sql");
        await ExecuteSqlScriptAsync(context, scriptPath, "SeedCatalog.sql", logger);
    }

    private static async Task ExecuteSqlScriptAsync(ApplicationDbContext context, string? scriptPath, string fileName, ILogger logger)
    {
        if (scriptPath is null)
        {
            logger.LogWarning("{File} was not found. Script was not applied.", fileName);
            return;
        }

        var script = await File.ReadAllTextAsync(scriptPath);
        var batches = SplitSqlBatches(script);

        foreach (var batch in batches)
        {
            await context.Database.ExecuteSqlRawAsync(batch);
        }
    }

    private static IEnumerable<string> SplitSqlBatches(string script)
    {
        var lines = script.Split('\n');
        var current = new System.Text.StringBuilder();

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd('\r');
            if (string.Equals(line.Trim(), "GO", StringComparison.OrdinalIgnoreCase))
            {
                var batch = current.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(batch))
                {
                    yield return batch;
                }
                current.Clear();
            }
            else
            {
                current.AppendLine(line);
            }
        }

        var last = current.ToString().Trim();
        if (!string.IsNullOrWhiteSpace(last))
        {
            yield return last;
        }
    }

    private static string? FindScriptPath(string folder, string fileName)
    {
        var baseDirectory = AppContext.BaseDirectory;
        var currentDirectory = Directory.GetCurrentDirectory();

        var candidates = new[]
        {
            Path.Combine(baseDirectory, folder, fileName),
            Path.Combine(currentDirectory, folder, fileName),
            Path.Combine(currentDirectory, "..", folder, fileName),
            Path.Combine(currentDirectory, "..", "ReservasXYZ.Infrastructure", folder, fileName),
            Path.Combine(currentDirectory, "ReservasXYZ.Infrastructure", folder, fileName),
            Path.Combine(currentDirectory, "..", "..", folder, fileName),
            Path.Combine(currentDirectory, "..", "..", "..", folder, fileName)
        };

        return candidates.Select(Path.GetFullPath).FirstOrDefault(File.Exists);
    }
}
