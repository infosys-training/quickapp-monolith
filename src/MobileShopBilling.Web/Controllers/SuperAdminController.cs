using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;

namespace MobileShopBilling.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class SuperAdminController : Controller
{
    private readonly ITenantService _tenantService;

    public SuperAdminController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboard = await _tenantService.GetSuperAdminDashboardAsync();
        return View(dashboard);
    }

    public async Task<IActionResult> Shops()
    {
        var shops = await _tenantService.GetAllTenantsAsync();
        return View(shops);
    }

    [HttpGet]
    public IActionResult CreateShop()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateShop(CreateTenantDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _tenantService.CreateTenantAsync(dto);
            TempData["Success"] = $"Shop '{dto.ShopName}' created successfully. Login: {dto.Email}";
            return RedirectToAction(nameof(Shops));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        await _tenantService.ToggleTenantStatusAsync(id);
        return RedirectToAction(nameof(Shops));
    }
}
