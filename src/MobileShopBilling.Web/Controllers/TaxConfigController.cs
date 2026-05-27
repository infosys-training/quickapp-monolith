using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;

namespace MobileShopBilling.Web.Controllers;

[Authorize(Roles = "ShopAdmin")]
public class TaxConfigController : Controller
{
    private readonly ITaxConfigurationService _taxService;

    public TaxConfigController(ITaxConfigurationService taxService)
    {
        _taxService = taxService;
    }

    public async Task<IActionResult> Index()
    {
        var taxes = await _taxService.GetAllAsync();
        return View(taxes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTaxConfigurationDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        await _taxService.CreateAsync(dto);
        TempData["Success"] = "Tax configuration created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var tax = await _taxService.GetByIdAsync(id);
        if (tax == null) return NotFound();

        var dto = new UpdateTaxConfigurationDto
        {
            Id = tax.Id,
            TaxName = tax.TaxName,
            Rate = tax.Rate,
            IsDefault = tax.IsDefault
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateTaxConfigurationDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        await _taxService.UpdateAsync(dto);
        TempData["Success"] = "Tax configuration updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _taxService.DeleteAsync(id);
        TempData["Success"] = "Tax configuration deleted.";
        return RedirectToAction(nameof(Index));
    }
}
