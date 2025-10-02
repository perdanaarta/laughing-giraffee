using System.Globalization;
using System.Text;
using Clems.Application.Extensions;
using Clems.Infrastructure.Extensions;
using Clems.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var culture = new CultureInfo("id-ID");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.AddIdentityServices();

builder.Services.AddControllersWithViews();
builder.Services.AddUnitOfWork();
builder.AddAppSwagger();
builder.AddAppAuth();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseAppSwagger();
}



app.UseHttpsRedirection();

app.UseRouting();
app.MapStaticAssets();

app.UseAppAuth();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();