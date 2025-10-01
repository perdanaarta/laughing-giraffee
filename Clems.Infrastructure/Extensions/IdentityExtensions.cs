using Clems.Domain.Model;
using Clems.Infrastructure.Data;
using Clems.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Clems.Infrastructure.Extensions;

public static class IdentityExtensions
{
    public static void AddIdentityServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        
        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
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