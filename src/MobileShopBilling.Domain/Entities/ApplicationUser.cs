using Microsoft.AspNetCore.Identity;

namespace MobileShopBilling.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
