using SharedKernel.Abstraction;

namespace Clems.Domain.Event;

public record TransactionAdded(Guid AccountId, decimal Debit, decimal Credit) : IDomainEvent;