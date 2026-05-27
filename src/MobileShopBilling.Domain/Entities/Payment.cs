using MobileShopBilling.Domain.Enums;

namespace MobileShopBilling.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string? TransactionReference { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}
