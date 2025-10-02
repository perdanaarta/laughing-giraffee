using Clems.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Clems.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .HasDiscriminator<string>("AccountType")
            .HasValue<Wallet>("Wallet")
            .HasValue<Debt>("Debt");

        modelBuilder.Entity<Account>()
            .HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Credit)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Debit)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasPrecision(18, 2);
    }
}