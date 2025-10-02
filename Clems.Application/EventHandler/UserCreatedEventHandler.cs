using Clems.Domain.Event;
using Clems.Domain.Model;
using Clems.Infrastructure;
using Clems.Infrastructure.Data;
using SharedKernel.Abstraction;

namespace Clems.Application.EventHandler;

public class UserCreatedEventHandler(AppDbContext dbContext, IUnitOfWork unitOfWork) : IEventHandler<UserCreated>
{
    public async Task HandleAsync(UserCreated @event, CancellationToken cancellationToken = default)
    {
        var wallet = new Wallet(@event.UserId, "Main");
        await dbContext.Accounts.AddAsync(wallet, cancellationToken);
        
        await unitOfWork.SaveAsync();
    }
}