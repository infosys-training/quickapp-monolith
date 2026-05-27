namespace MobileShopBilling.Domain.Entities;

public class Product : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? IMEI { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; } = 5;
    public bool IsActive { get; set; } = true;
}
