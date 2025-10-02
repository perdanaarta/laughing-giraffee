using Clems.Application.EventHandler;
using Clems.Application.Services;
using Clems.Domain.Event;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstraction;
using SharedKernel.Mediator;

namespace Clems.Application.Extensions;

public static class ServicesExtensions
{
    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<WalletService>();
        builder.Services.AddScoped<TransactionService>();
        builder.Services.AddScoped<DebtService>();

        builder.Services.AddTransient<IEventHandler<UserCreated>, UserCreatedEventHandler>();

        builder.Services.AddScoped<IMediator, Mediator>();
    }
}