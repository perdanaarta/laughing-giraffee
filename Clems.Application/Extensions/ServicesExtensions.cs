using Clems.Application.EventHandler;
using Clems.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstraction;

namespace Clems.Application.Extensions;

public static class ServicesExtensions
{
    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<WalletService>();
        builder.Services.AddScoped<TransactionService>();
        
        builder.Services.AddScoped<UserCreatedEventHandler>();
        
        builder.Services.AddScoped<IMediator, Mediator>();
    }   
}