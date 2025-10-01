using Clems.Domain.Abstraction;
using SharedKernel.Abstraction;

namespace Clems.Domain.Model;

public record TransactionAdded(Guid AccountId, decimal Debit, decimal Credit) : IDomainEvent;


public abstract class Account : Aggregate
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid UserId { get; protected set; }
    public string Name { get; protected set; }
    public decimal Balance { get; protected set; }

    protected readonly List<Transaction> _transactions = new();
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    protected Account(Guid userId, string name)
    {
        UserId = userId;
        Name = name;
    }

    public void AddTransaction(Transaction transaction)
    {
        ApplyTransaction(transaction);
        _transactions.Add(transaction);
        AddDomainEvent(new TransactionAdded(Id, transaction.Debit, transaction.Credit));
    }

    protected abstract void ApplyTransaction(Transaction transaction);
}

public class Wallet : Account
{
    public Wallet(Guid userId, string name) : base(userId, name) { }

    protected override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Debit > Balance)
            throw new InvalidOperationException("Insufficient funds");

        // Asset behavior: Credit increases, Debit decreases
        Balance += transaction.Credit - transaction.Debit;
    }
}

public class Debt : Account
{
    public Debt(Guid userId, string name) : base(userId, name) { }

    protected override void ApplyTransaction(Transaction transaction)
    {
        // Liability behavior: Credit increases debt, Debit repays debt
        Balance += transaction.Credit - transaction.Debit;

        if (Balance > 0)
            throw new InvalidOperationException("Debt balance cannot be positive (overpaid)");
    }
}