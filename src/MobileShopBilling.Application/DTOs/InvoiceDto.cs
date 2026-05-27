using MobileShopBilling.Domain.Enums;

namespace MobileShopBilling.Application.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
    public PaymentMode? PaymentMode { get; set; }
    public string? TransactionReference { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}

public class InvoiceItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateInvoiceDto
{
    public Guid? CustomerId { get; set; }
    public string? Notes { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string? TransactionReference { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}

public class CreateInvoiceItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxPercent { get; set; }
}
