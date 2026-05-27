using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;

namespace MobileShopBilling.Web.Controllers;

[Authorize(Roles = "ShopAdmin")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    private Guid GetTenantId()
    {
        var tenantClaim = User.FindFirst("TenantId")?.Value;
        return Guid.TryParse(tenantClaim, out var id) ? id : Guid.Empty;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync(GetTenantId());
        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        try
        {
            await _userService.CreateAsync(dto, GetTenantId());
            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        var dto = new UpdateUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            IsActive = user.IsActive
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        try
        {
            await _userService.UpdateAsync(dto);
            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await _userService.DeleteAsync(id);
        TempData["Success"] = "User deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }
}
