using Clems.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clems.Web.Controllers;

[Route("[controller]/[action]")]
public class WalletController(WalletService walletService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var wallets = await walletService.FindAllAsync();
        return View(wallets);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WalletCreateDto dto)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        var wallet = await walletService.Create(dto);
        return Json(new { success = true, wallet.Id, wallet.Name, wallet.Balance });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var wallet = await walletService.FindByIdAsync(id);
        if (wallet == null) return NotFound();

        return Json(wallet);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await walletService.Delete(id);
        return Json(new { success = true });
    }
    
    [HttpPost("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] WalletUpdateDto dto)
    {
        await walletService.Update(id, dto.Name);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Deposit([FromBody] WalletTransactionDto dto)
    {
        await walletService.Deposit(dto.WalletId, dto.Amount, dto.Description);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Withdraw([FromBody] WalletTransactionDto dto)
    {
        await walletService.Withdraw(dto.WalletId, dto.Amount, dto.Description);
        return Json(new { success = true });
    }

}

public record WalletTransactionDto(Guid WalletId, decimal Amount, string Description);

public class WalletUpdateDto 
{ 
    public string Name { get; set; } 
}
