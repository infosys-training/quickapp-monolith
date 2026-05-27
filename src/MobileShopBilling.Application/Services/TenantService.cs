using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;
using MobileShopBilling.Domain.Entities;
using MobileShopBilling.Domain.Interfaces;

namespace MobileShopBilling.Application.Services;

public class TenantService : ITenantService
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<TaxConfiguration> _taxRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantProvider _tenantProvider;

    public TenantService(
        IRepository<Tenant> tenantRepository,
        IRepository<TaxConfiguration> taxRepository,
        UserManager<ApplicationUser> userManager,
        ITenantProvider tenantProvider)
    {
        _tenantRepository = tenantRepository;
        _taxRepository = taxRepository;
        _userManager = userManager;
        _tenantProvider = tenantProvider;
    }

    public async Task<TenantDto> CreateTenantAsync(CreateTenantDto dto)
    {
        var tenant = new Tenant
        {
            ShopName = dto.ShopName,
            OwnerName = dto.OwnerName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            GstNumber = dto.GstNumber
        };

        await _tenantRepository.AddAsync(tenant);

        var adminUser = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.OwnerName,
            TenantId = tenant.Id,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(adminUser, dto.AdminPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        await _userManager.AddToRoleAsync(adminUser, "ShopAdmin");

        _tenantProvider.SetTenantId(tenant.Id);

        var defaultTaxes = new[]
        {
            new TaxConfiguration { TaxName = "GST 18%", Rate = 18, IsDefault = true, TenantId = tenant.Id },
            new TaxConfiguration { TaxName = "GST 12%", Rate = 12, IsDefault = false, TenantId = tenant.Id },
            new TaxConfiguration { TaxName = "GST 5%", Rate = 5, IsDefault = false, TenantId = tenant.Id },
            new TaxConfiguration { TaxName = "GST 28%", Rate = 28, IsDefault = false, TenantId = tenant.Id }
        };

        foreach (var tax in defaultTaxes)
        {
            await _taxRepository.AddAsync(tax);
        }

        return MapToDto(tenant);
    }

    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
    {
        var tenants = await _tenantRepository.GetAllAsync();
        return tenants.Select(MapToDto);
    }

    public async Task<TenantDto?> GetTenantByIdAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        return tenant == null ? null : MapToDto(tenant);
    }

    public async Task ToggleTenantStatusAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Tenant not found");

        tenant.IsActive = !tenant.IsActive;
        tenant.DeactivatedAt = tenant.IsActive ? null : DateTime.UtcNow;

        await _tenantRepository.UpdateAsync(tenant);
    }

    public async Task<SuperAdminDashboardDto> GetSuperAdminDashboardAsync()
    {
        var tenants = (await _tenantRepository.GetAllAsync()).ToList();

        var users = await _userManager.Users.CountAsync();

        return new SuperAdminDashboardDto
        {
            TotalShops = tenants.Count,
            ActiveShops = tenants.Count(t => t.IsActive),
            InactiveShops = tenants.Count(t => !t.IsActive),
            TotalUsers = users,
            RecentShops = tenants.OrderByDescending(t => t.CreatedAt).Take(10).Select(MapToDto).ToList()
        };
    }

    private static TenantDto MapToDto(Tenant t) => new()
    {
        Id = t.Id,
        ShopName = t.ShopName,
        OwnerName = t.OwnerName,
        Email = t.Email,
        Phone = t.Phone,
        Address = t.Address,
        GstNumber = t.GstNumber,
        IsActive = t.IsActive,
        CreatedAt = t.CreatedAt
    };
}
