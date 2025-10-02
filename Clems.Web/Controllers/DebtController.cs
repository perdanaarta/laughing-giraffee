using Clems.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clems.Web.Controllers;

[Route("[controller]/[action]")]
public class DebtController(DebtService debtService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var debts = await debtService.FindAllAsync();
        return View(debts);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DebtCreateDto dto)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        var debt = await debtService.Create(dto);
        return Json(new { success = true, debt.Id, debt.Name, debt.Balance });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var debt = await debtService.FindByIdAsync(id);
        if (debt == null) return NotFound();

        return Json(debt);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await debtService.Delete(id); // or call a dedicated Delete
        return Json(new { success = true });
    }
    
    [HttpPost("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DebtUpdateDto dto)
    {
        await debtService.Update(id, dto.Name);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Borrow([FromBody] DebtTransactionDto dto)
    {
        await debtService.Borrow(dto.DebtId, dto.Amount, dto.Description);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Repay([FromBody] DebtTransactionDto dto)
    {
        await debtService.Repay(dto.DebtId, dto.Amount, dto.Description);
        return Json(new { success = true });
    }

}


public record DebtTransactionDto(Guid DebtId, decimal Amount, string Description);

public class DebtUpdateDto 
{ 
    public string Name { get; set; } 
}