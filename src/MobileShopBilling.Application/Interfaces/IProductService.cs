using MobileShopBilling.Application.DTOs;

namespace MobileShopBilling.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task UpdateAsync(UpdateProductDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();
    Task<IEnumerable<ProductDto>> SearchAsync(string term);
}
