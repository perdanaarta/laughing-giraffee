using SharedKernel.Abstraction;

namespace Clems.Domain.Event;

public record UserCreated(Guid UserId) : IDomainEvent;