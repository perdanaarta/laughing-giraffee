using System.Security.Claims;
using Clems.Domain.Model;
using Clems.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clems.Application.Services;

public class TransactionService(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
{
    private Guid GetUserId()
    {
        var user = contextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new InvalidOperationException("No authenticated user found in the current request.");

        return Guid.Parse(userIdClaim);
    }

    public async Task<List<TransactionHistoryDto>> History(Guid accountId)
    {
        var userId = GetUserId();

        var transactions = await dbContext.Transactions
            .Include(t => t.Account)
            .Where(t => t.AccountId == accountId)
            .Where(w => w.Account.UserId == userId)
            .OrderBy(t => t.Date)
            .ToListAsync();

        var history = new List<TransactionHistoryDto>();
        decimal runningBalance = 0;

        foreach (var t in transactions)
        {
            var signedAmount = t.Account switch
            {
                Wallet => t.Credit - t.Debit, // +deposit / -withdraw
                Debt => t.Debit - t.Credit, // +repayment / -borrowed
                _ => t.Credit - t.Debit
            };

            runningBalance += signedAmount;

            history.Add(new TransactionHistoryDto
            {
                Date = t.Date,
                Description = t.Description,
                Amount = signedAmount,
                BalanceAfter = runningBalance,
                AccountType = t.Account.GetType().Name
            });
        }

        return history;
    }

    public async Task<List<TransactionSummaryDto>> Recapitulate()
    {
        var userId = GetUserId();

        var accounts = await dbContext.Accounts
            .Include(a => a.Transactions)
            .Where(a => a.UserId == userId)
            .ToListAsync();

        var summaries = new List<TransactionSummaryDto>();

        foreach (var account in accounts)
        {
            decimal total = 0;

            foreach (var t in account.Transactions)
                total += account switch
                {
                    Wallet => t.Credit - t.Debit,
                    Debt => t.Debit - t.Credit,
                    _ => t.Credit - t.Debit
                };

            summaries.Add(new TransactionSummaryDto
            {
                AccountId = account.Id,
                AccountType = account.GetType().Name,
                TotalChange = total,
                Balance = account.Balance,
                TransactionCount = account.Transactions.Count
            });
        }

        return summaries;
    }
}

public class TransactionHistoryDto
{
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; } // signed: + or -
    public decimal BalanceAfter { get; set; }
    public string AccountType { get; set; }
}

public class TransactionSummaryDto
{
    public Guid AccountId { get; set; }
    public string AccountType { get; set; }
    public decimal TotalChange { get; set; } // total +/- over all transactions
    public decimal Balance { get; set; } // current balance
    public int TransactionCount { get; set; }
}