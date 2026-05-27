namespace MobileShopBilling.Domain.Entities;

public class Supplier : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GstNumber { get; set; }
    public bool IsActive { get; set; } = true;
}
