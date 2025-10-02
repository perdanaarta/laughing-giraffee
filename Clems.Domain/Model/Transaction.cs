namespace Clems.Domain.Model;

public class Transaction
{
    public Transaction(decimal credit, decimal debit, string description)
    {
        Id = Guid.CreateVersion7();
        Credit = credit;
        Debit = debit;
        Description = description;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public decimal Credit { get; private set; }
    public decimal Debit { get; private set; }
    public string Description { get; private set; }
    public DateTime Date { get; private set; } = DateTime.UtcNow;

    // FK to Account
    public Guid AccountId { get; set; }
    public virtual Account Account { get; set; }
}