using SharedKernel.Abstraction;

namespace Clems.Domain.Event;

public record AccountDeleted(Guid AccountId) : IDomainEvent;

public record AccountModified(Guid AccountId) : IDomainEvent;