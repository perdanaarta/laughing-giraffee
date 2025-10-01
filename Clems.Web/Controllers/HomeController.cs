using System.Diagnostics;
using Clems.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Clems.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace Clems.Web.Controllers;


public class HomeController : Controller
{
    private readonly WalletService _walletService;
    private readonly TransactionService _transactionService;

    public HomeController(WalletService walletService, TransactionService transactionService)
    {
        _walletService = walletService;
        _transactionService = transactionService;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var wallets = await _walletService.FindAllAsync();
        return View(wallets);
    }

    [HttpGet]
    public async Task<IActionResult> WalletHistory(Guid walletId)
    {
        var history = await _transactionService.History(walletId);
        return PartialView("_WalletHistory", history);
    }

    [HttpPost]
    public async Task<IActionResult> AddTransaction(Guid walletId, decimal amount, string type, string description)
    {
        if (type == "Deposit")
            await _walletService.Deposit(walletId, amount, description);
        else if (type == "Withdraw")
            await _walletService.Withdraw(walletId, amount, description);

        var history = await _transactionService.History(walletId);
        return PartialView("_WalletHistory", history);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] WalletCreateDto dto)
    {
        var wallet = await _walletService.Create(dto);
        return Json(new { id = wallet.Id, name = wallet.Name, balance = wallet.Balance.ToString("C") });
    }
}