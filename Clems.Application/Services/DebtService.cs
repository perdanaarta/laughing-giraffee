using System.Security.Claims;
using Clems.Domain.Model;
using Clems.Infrastructure;
using Clems.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Clems.Application.Services;

public record DebtCreateDto(string Name, decimal InitialDebt);

public record BorrowRepayResult(decimal NewBalance, Transaction Transaction);

public record DebtDto(Guid Id, string Name, decimal Amount);

public class DebtService(AppDbContext dbContext, IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
{
    private Guid GetUserId()
    {
        var user = contextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new InvalidOperationException("No authenticated user found in the current request.");

        return Guid.Parse(userIdClaim);
    }

    public async Task<List<DebtDto>> FindAllAsync()
    {
        var userId = GetUserId();
        return await dbContext.Accounts
            .OfType<Debt>()
            .Include(d => d.Transactions)
            .Where(d => d.UserId == userId)
            .Select(d => new DebtDto(d.Id, d.Name, d.Balance))
            .ToListAsync();
    }

    public async Task<DebtDto?> FindByIdAsync(Guid debtId)
    {
        var userId = GetUserId();
        return await dbContext.Accounts
            .OfType<Debt>()
            .Include(d => d.Transactions)
            .Where(d => d.UserId == userId)
            .Where(d => d.Id == debtId)
            .Select(d => new DebtDto(d.Id, d.Name, d.Balance))
            .FirstOrDefaultAsync();
    }

    public async Task<Debt> Create(DebtCreateDto dto)
    {
        var userId = GetUserId();
        var debt = new Debt(userId, dto.Name);

        if (dto.InitialDebt > 0) debt.AddTransaction(new Transaction(dto.InitialDebt, 0, "Initial debt"));

        dbContext.Accounts.Add(debt);

        await unitOfWork.SaveAsync();
        return debt;
    }

    public async Task Update(Guid debtId, string name)
    {
        var userId = GetUserId();
        var debt = await dbContext.Accounts
            .OfType<Debt>()
            .Include(d => d.Transactions)
            .FirstOrDefaultAsync(d => d.Id == debtId && d.UserId == userId);
        
        if (debt == null)
            throw new Exception("Debt not found");
        
        debt.Update(name);
        await unitOfWork.SaveAsync();
    }

    public async Task Delete(Guid debtId)
    {
        var userId = GetUserId();
        var debt = await dbContext.Accounts
            .OfType<Debt>()
            .Include(d => d.Transactions)
            .FirstOrDefaultAsync(d => d.Id == debtId && d.UserId == userId);
        
        if (debt == null)
            throw new Exception("Debt not found");
        
        debt.Delete();
        dbContext.Accounts.Remove(debt);
        await unitOfWork.SaveAsync();
    }
    
    public async Task<BorrowRepayResult> Borrow(Guid debtId, decimal amount, string description = "Borrow")
    {
        var userId = GetUserId();
        var debt = await dbContext.Accounts
            .OfType<Debt>()
            .Include(d => d.Transactions)
            .FirstOrDefaultAsync(d => d.Id == debtId && d.UserId == userId);

        if (debt == null)
            throw new Exception("Debt not found");

        var transaction = new Transaction(amount, 0, description);
        debt.AddTransaction(transaction);

        dbContext.Transactions.Add(transaction);

        await unitOfWork.SaveAsync();

        return new BorrowRepayResult(debt.Balance, transaction);
    }

    public async Task<BorrowRepayResult> Repay(Guid debtId, decimal amount, string description = "Repayment")
    {
        var userId = GetUserId();
        var debt = await dbContext.Accounts
            .OfType<Debt>()
            .Include(d => d.Transactions)
            .FirstOrDefaultAsync(d => d.Id == debtId && d.UserId == userId);

        if (debt == null)
            throw new Exception("Debt not found");

        var transaction = new Transaction(0, amount, description);
        debt.AddTransaction(transaction);

        dbContext.Transactions.Add(transaction);

        await unitOfWork.SaveAsync();

        return new BorrowRepayResult(debt.Balance, transaction);
    }

    public async Task DeleteTransaction(Guid debtId, Guid transactionId)
    {
        var userId = GetUserId();
        var debt = await dbContext.Accounts
            .OfType<Debt>()
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == debtId && w.UserId == userId);

        if (debt == null)
            throw new Exception("Wallet not found");

        debt.DeleteTransaction(transactionId);
        dbContext.Transactions.Remove(debt.Transactions.First(t => t.Id == transactionId));

        await unitOfWork.SaveAsync();
    }
}
