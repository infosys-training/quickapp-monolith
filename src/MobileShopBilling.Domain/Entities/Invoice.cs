using MobileShopBilling.Domain.Enums;

namespace MobileShopBilling.Domain.Entities;

public class Invoice : TenantEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string? Notes { get; set; }
    public List<InvoiceItem> Items { get; set; } = new();
    public Payment? Payment { get; set; }
}
