namespace MobileShopBilling.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? IMEI { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public bool IsActive { get; set; }
    public bool IsLowStock => StockQuantity <= LowStockThreshold;
}

public class CreateProductDto
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
}

public class UpdateProductDto : CreateProductDto
{
    public Guid Id { get; set; }
}
