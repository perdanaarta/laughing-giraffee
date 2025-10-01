using SharedKernel.Abstraction;

namespace Clems.Domain.Event;

public record UserCreatedEvent(Guid UserId) : IDomainEvent;