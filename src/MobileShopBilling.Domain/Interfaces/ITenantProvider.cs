namespace MobileShopBilling.Domain.Interfaces;

public interface ITenantProvider
{
    Guid? GetCurrentTenantId();
    void SetTenantId(Guid tenantId);
}
