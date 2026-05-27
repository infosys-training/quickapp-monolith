using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;
using MobileShopBilling.Domain.Entities;
using MobileShopBilling.Domain.Interfaces;

namespace MobileShopBilling.Application.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Brand = dto.Brand,
            IMEI = dto.IMEI,
            Barcode = dto.Barcode,
            Description = dto.Description,
            Category = dto.Category,
            PurchasePrice = dto.PurchasePrice,
            SellingPrice = dto.SellingPrice,
            StockQuantity = dto.StockQuantity,
            LowStockThreshold = dto.LowStockThreshold
        };

        await _repository.AddAsync(product);
        return MapToDto(product);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task UpdateAsync(UpdateProductDto dto)
    {
        var product = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Product not found");

        product.Name = dto.Name;
        product.Brand = dto.Brand;
        product.IMEI = dto.IMEI;
        product.Barcode = dto.Barcode;
        product.Description = dto.Description;
        product.Category = dto.Category;
        product.PurchasePrice = dto.PurchasePrice;
        product.SellingPrice = dto.SellingPrice;
        product.StockQuantity = dto.StockQuantity;
        product.LowStockThreshold = dto.LowStockThreshold;

        await _repository.UpdateAsync(product);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
    {
        var products = await _repository.FindAsync(p => p.StockQuantity <= p.LowStockThreshold && p.IsActive);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string term)
    {
        var lower = term.ToLower();
        var products = await _repository.FindAsync(p =>
            p.Name.ToLower().Contains(lower) ||
            (p.Brand != null && p.Brand.ToLower().Contains(lower)) ||
            (p.Barcode != null && p.Barcode.ToLower().Contains(lower)) ||
            (p.IMEI != null && p.IMEI.ToLower().Contains(lower)));
        return products.Select(MapToDto);
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Brand = p.Brand,
        IMEI = p.IMEI,
        Barcode = p.Barcode,
        Description = p.Description,
        Category = p.Category,
        PurchasePrice = p.PurchasePrice,
        SellingPrice = p.SellingPrice,
        StockQuantity = p.StockQuantity,
        LowStockThreshold = p.LowStockThreshold,
        IsActive = p.IsActive
    };
}
