using MobileShopBilling.Application.DTOs;

namespace MobileShopBilling.Application.Interfaces;

public interface ITenantService
{
    Task<TenantDto> CreateTenantAsync(CreateTenantDto dto);
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
    Task<TenantDto?> GetTenantByIdAsync(Guid id);
    Task ToggleTenantStatusAsync(Guid id);
    Task<SuperAdminDashboardDto> GetSuperAdminDashboardAsync();
}
