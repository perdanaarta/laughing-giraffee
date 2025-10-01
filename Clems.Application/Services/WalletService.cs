using System.Security.Claims;
using Clems.Domain.Model;
using Clems.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;

namespace Clems.Application.Services;

public record WalletCreateDto(string Name, decimal InitialBalance);

public class WalletService(AppDbContext dbContext, IMediator mediator, IHttpContextAccessor contextAccessor)
{
    private Guid GetUserId()
    {
        var user = contextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new InvalidOperationException("No authenticated user found in the current request.");

        return Guid.Parse(userIdClaim);
    }
    
    public async Task<List<Wallet>> FindAllAsync()
    {
        var userId = GetUserId();
        return await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }
    
    public async Task<Wallet?> FindByIdAsync(Guid walletId)
    {
        var userId = GetUserId();
        return await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .Where(w => w.UserId == userId)
            .FirstOrDefaultAsync(w => w.Id == walletId);
    }
    
    public async Task<Wallet> Create(WalletCreateDto dto)
    {
        var userId = GetUserId();
        var wallet = new Wallet(userId, dto.Name);

        if (dto.InitialBalance > 0)
        {
            wallet.AddTransaction(new Transaction(credit: dto.InitialBalance, debit: 0, description: "Initial balance"));
        }

        dbContext.Accounts.Add(wallet);
        
        foreach (var e in wallet.ClearDomainEvents())
            await mediator.PublishAsync(e);
        
        await dbContext.SaveChangesAsync();
        return wallet;
    }
    
    public async Task Deposit(Guid walletId, decimal amount, string description = "Deposit")
    {
        var userId = GetUserId();

        var wallet = await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);

        if (wallet == null)
            throw new Exception("Wallet not found");

        var transaction = new Transaction(credit: amount, debit: 0, description);
        
        wallet.AddTransaction(transaction);
        
        dbContext.Transactions.Add(transaction);

        // Publish domain events
        foreach (var e in wallet.ClearDomainEvents())
            await mediator.PublishAsync(e);

        await dbContext.SaveChangesAsync();
    }
    
    public async Task Withdraw(Guid walletId, decimal amount, string description = "Withdrawal")
    {
        var userId = GetUserId();
        var wallet = await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .Where(w => w.UserId == userId)
            .FirstOrDefaultAsync(w => w.Id == walletId);

        if (wallet == null)
            throw new Exception("Wallet not found");

        var transaction = new Transaction(credit: 0, debit: amount, description);
        wallet.AddTransaction(transaction);
        
        dbContext.Transactions.Add(transaction);
        
        foreach (var e in wallet.ClearDomainEvents())
            await mediator.PublishAsync(e);
        
        await dbContext.SaveChangesAsync();
    }
    
    public async Task Transfer(Guid fromWalletId, Guid toWalletId, decimal amount, string description = "Transfer")
    {
        var userId = GetUserId();
        
        if (fromWalletId == toWalletId)
            throw new InvalidOperationException("Cannot transfer to the same wallet");

        var wallets = await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .Where(w => w.UserId == userId)
            .Where(w => w.Id == fromWalletId || w.Id == toWalletId)
            .ToListAsync();

        var srcWallet = wallets.FirstOrDefault(w => w.Id == fromWalletId);
        var dstWallet = wallets.FirstOrDefault(w => w.Id == toWalletId);

        if (srcWallet == null || dstWallet == null)
            throw new Exception("One or both wallets not found");
        
        srcWallet.AddTransaction(new Transaction(credit: 0, debit: amount, description));
        dstWallet.AddTransaction(new Transaction(credit: amount, debit: 0, description));


        foreach (var wallet in new[] { srcWallet, dstWallet })
        {
            foreach (var e in wallet.ClearDomainEvents())
                await mediator.PublishAsync(e);
        }
        await dbContext.SaveChangesAsync();
    }
}