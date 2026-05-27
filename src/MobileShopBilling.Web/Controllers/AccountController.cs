using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Domain.Entities;

namespace MobileShopBilling.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !user.IsActive)
        {
            ModelState.AddModelError("", "Invalid login attempt or account is deactivated.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>();

            if (user.TenantId.HasValue)
                claims.Add(new Claim("TenantId", user.TenantId.Value.ToString()));

            if (claims.Count > 0)
            {
                await _userManager.AddClaimsAsync(user, claims);
                await _signInManager.RefreshSignInAsync(user);
            }

            if (roles.Contains("SuperAdmin"))
                return RedirectToAction("Index", "SuperAdmin");

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
