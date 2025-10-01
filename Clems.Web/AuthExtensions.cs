using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Clems.Web;

public static class AuthExtensions
{
    public static void AddAppAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme) 
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/AccessDenied";
            });

        builder.Services.AddAuthorization();
    }

    public static void UseAppAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}