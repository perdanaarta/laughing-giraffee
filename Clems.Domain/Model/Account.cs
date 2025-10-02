using Clems.Domain.Abstraction;
using Clems.Domain.Event;

namespace Clems.Domain.Model;

public abstract class Account : Aggregate
{
    protected readonly List<Transaction> _transactions = new();

    protected Account(Guid userId, string name)
    {
        UserId = userId;
        Name = name;
    }

    public Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid UserId { get; protected set; }
    public string Name { get; protected set; }
    public decimal Balance { get; protected set; }
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    public void AddTransaction(Transaction transaction)
    {
        ApplyTransaction(transaction);
        _transactions.Add(transaction);
        AddDomainEvent(new TransactionAdded(Id, transaction.Debit, transaction.Credit));
    }

    public void DeleteTransaction(Guid transactionId)
    {
        var tx = _transactions.SingleOrDefault(t => t.Id == transactionId);
        if (tx == null) return;

        _transactions.Remove(tx);

        // Recalculate from scratch
        Balance = 0;
        foreach (var transaction in _transactions)
            ApplyTransaction(transaction);
    }

    public void Delete()
    {
        AddDomainEvent(new AccountDeleted(Id));
    }

    public void Update(string name)
    {
        Name = name;
        AddDomainEvent(new AccountModified(Id));
    }

    protected abstract void ApplyTransaction(Transaction transaction);
}

public class Wallet : Account
{
    public Wallet(Guid userId, string name) : base(userId, name)
    {
    }

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
    public Debt(Guid userId, string name) : base(userId, name)
    {
    }

    protected override void ApplyTransaction(Transaction transaction)
    {
        // Liability behavior: Credit increases debt, Debit repays debt
        Balance += transaction.Debit - transaction.Credit;
    }
}