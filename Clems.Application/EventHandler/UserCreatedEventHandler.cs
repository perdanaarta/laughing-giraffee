using Clems.Application.Services;
using Clems.Domain.Event;
using Clems.Domain.Model;
using Clems.Infrastructure.Data;
using SharedKernel.Abstraction;

namespace Clems.Application.EventHandler;

public class UserCreatedEventHandler(AppDbContext dbContext) : IEventHandler<UserCreatedEvent>
{
    public async Task HandleAsync(UserCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        var wallet = new Wallet(@event.UserId, "Main");
        dbContext.Accounts.Add(wallet);
    }
}