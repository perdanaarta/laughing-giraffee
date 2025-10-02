using Clems.Domain.Model;
using Clems.Infrastructure.Data;
using Clems.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clems.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var appDb = services.GetRequiredService<AppDbContext>();
            appDb.Database.Migrate(); // Auto-migrate AppDbContext

            var identityDb = services.GetRequiredService<IdentityContext>();
            identityDb.Database.Migrate(); // Auto-migrate IdentityContext
        }
        catch (Exception ex)
        {
            // Log the error properly in real apps
            Console.WriteLine($"Migration failed: {ex.Message}");
            throw;
        }
    }
    
    public static void AddInfrastructures(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

        //     builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
        //         {
        //             options.Password.RequireDigit = false;
        //             options.Password.RequireLowercase = false;
        //             options.Password.RequireUppercase = false;
        //             options.Password.RequireNonAlphanumeric = false;
        //             options.Password.RequiredLength = 1;
        //             options.Password.RequiredUniqueChars = 1;
        //         })
        //         .AddEntityFrameworkStores<IdentityContext>()
        //         .AddSignInManager<SignInManager<User>>() 
        //         .AddDefaultTokenProviders();
    }
}