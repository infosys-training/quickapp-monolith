namespace MobileShopBilling.Domain.Entities;

public class Tenant : BaseEntity
{
    public string ShopName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? GstNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeactivatedAt { get; set; }
}
