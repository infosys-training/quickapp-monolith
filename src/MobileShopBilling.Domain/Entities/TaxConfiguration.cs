namespace MobileShopBilling.Domain.Entities;

public class TaxConfiguration : TenantEntity
{
    public string TaxName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
}
