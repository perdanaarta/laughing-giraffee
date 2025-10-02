using Clems.Domain.Abstraction;
using Clems.Infrastructure.Data;
using Clems.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Mediator;

namespace Clems.Infrastructure;

public class UnitOfWork(AppDbContext appDb, IdentityContext identityDb, IMediator mediator)
    : IUnitOfWork
{
    public async Task SaveAsync()
    {
        var dbContexts = new DbContext[] { appDb, identityDb };

        var aggregates = dbContexts
            .SelectMany(db => db.ChangeTracker
                .Entries<Aggregate>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity))
            .ToList();

        var domainEvents = aggregates.SelectMany(a => a.ClearDomainEvents()).ToList();

        foreach (var e in domainEvents)
            await mediator.PublishAsync(e);
        
        foreach (var dbContext in dbContexts)
            await dbContext.SaveChangesAsync();

    }
}