using Clems.Application.Services;
using Clems.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clems.Web.Controllers;


[Authorize]
public class HomeController(    
    WalletService walletService,
    DebtService debtService,
    TransactionService transactionService
) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var wallets = await walletService.FindAllAsync();
        var debts = await debtService.FindAllAsync();
        var recaps = await transactionService.Recapitulate();

        var vm = new DashboardViewModel(wallets, debts, recaps);
        return View(vm);
    }
    
    [HttpGet("AccountsPartial")]
    public async Task<IActionResult> AccountsPartial()
    {
        var wallets = await walletService.FindAllAsync();
        var debts = await debtService.FindAllAsync();
        var recaps = await transactionService.Recapitulate();

        var vm = new DashboardViewModel(wallets, debts, recaps);
        return PartialView("_AccountsPartial", vm);
    }
}

