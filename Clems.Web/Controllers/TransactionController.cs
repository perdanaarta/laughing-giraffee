using Clems.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clems.Web.Controllers;

public class TransactionController(TransactionService transactionService) : Controller
{
    public async Task<IActionResult> History(Guid accountId)
    {
        var history = await transactionService.History(accountId);
        return PartialView("_TransactionHistory", history); 
    }
    
    [HttpGet]
    public async Task<IActionResult> HistoryJson(Guid accountId)
    {
        var history = await transactionService.History(accountId);
        var balance = history.LastOrDefault()?.BalanceAfter ?? 0;

        return Json(new
        {
            transactions = history.Select(t => new
            {
                date = t.Date,
                description = t.Description,
                amount = t.Amount,
                balanceAfter = t.BalanceAfter
            }),
            balance
        });
    }

    public async Task<IActionResult> Recap()
    {
        var recap = await transactionService.Recapitulate();
        return View(recap);
    }
}