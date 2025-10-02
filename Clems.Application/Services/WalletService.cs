using System.Security.Claims;
using Clems.Application.Exceptions;
using Clems.Domain.Model;
using Clems.Infrastructure;
using Clems.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Clems.Application.Services;

public record WalletCreateDto(string Name, decimal InitialBalance);

public record WalletDto(Guid Id, string Name, decimal Balance);

public class WalletService(AppDbContext dbContext, IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
{
    private Guid GetUserId()
    {
        var user = contextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new InvalidOperationException("No authenticated user found in the current request.");

        return Guid.Parse(userIdClaim);
    }

    public async Task<List<WalletDto>> FindAllAsync()
    {
        var userId = GetUserId();
        return await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .Where(w => w.UserId == userId)
            .Select(w => new WalletDto(w.Id, w.Name, w.Balance))
            .ToListAsync();
    }

    public async Task<WalletDto?> FindByIdAsync(Guid walletId)
    {
        var userId = GetUserId();
        return await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .Where(w => w.UserId == userId)
            .Where(w => w.Id == walletId)
            .Select(w => new WalletDto(w.Id, w.Name, w.Balance))
            .FirstOrDefaultAsync();
    }

    public async Task<Wallet> Create(WalletCreateDto dto)
    {
        var userId = GetUserId();
        var wallet = new Wallet(userId, dto.Name);

        if (dto.InitialBalance > 0) wallet.AddTransaction(new Transaction(dto.InitialBalance, 0, "Initial balance"));

        dbContext.Accounts.Add(wallet);

        await unitOfWork.SaveAsync();
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

        var transaction = new Transaction(amount, 0, description);

        wallet.AddTransaction(transaction);

        dbContext.Transactions.Add(transaction);

        await unitOfWork.SaveAsync();
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

        var transaction = new Transaction(0, amount, description);
        wallet.AddTransaction(transaction);

        dbContext.Transactions.Add(transaction);

        await unitOfWork.SaveAsync();
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

        srcWallet.AddTransaction(new Transaction(0, amount, description));
        dstWallet.AddTransaction(new Transaction(amount, 0, description));

        await unitOfWork.SaveAsync();
    }

    public async Task DeleteTransaction(Guid walletId, Guid transactionId)
    {
        var userId = GetUserId();
        var wallet = await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);

        if (wallet == null)
            throw new Exception("Wallet not found");

        wallet.DeleteTransaction(transactionId);
        dbContext.Transactions.Remove(wallet.Transactions.First(t => t.Id == transactionId));

        await unitOfWork.SaveAsync();
    }

    public async Task Update(Guid walletId, string name)
    {
        var userId = GetUserId();
        var wallet = await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);

        if (wallet == null)
            throw new WalletNotFoundException(walletId);

        wallet.Update(name);

        await unitOfWork.SaveAsync();
    }

    public async Task Delete(Guid walletId)
    {
        var userId = GetUserId();
        var wallet = await dbContext.Accounts
            .OfType<Wallet>()
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);

        if (wallet == null)
            throw new WalletNotFoundException(walletId);

        wallet.Delete();
        dbContext.Accounts.Remove(wallet);

        await unitOfWork.SaveAsync();
    }
}