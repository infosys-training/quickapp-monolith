using MobileShopBilling.Domain.Interfaces;

namespace MobileShopBilling.Infrastructure.Services;

public class TenantProvider : ITenantProvider
{
    private Guid? _tenantId;

    public Guid? GetCurrentTenantId() => _tenantId;

    public void SetTenantId(Guid tenantId)
    {
        _tenantId = tenantId;
    }
}
