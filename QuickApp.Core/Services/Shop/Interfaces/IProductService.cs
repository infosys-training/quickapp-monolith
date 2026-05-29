using QuickApp.Core.Services.Shop.HttpClients;

namespace QuickApp.Core.Services.Shop
{
    public interface IProductService
    {
        Task<IList<ProductServiceProductDto>> GetAllProductsAsync();
        Task<ProductServiceProductDto?> GetProductByIdAsync(int id);
        Task<ProductServiceProductDto> CreateProductAsync(CreateProductServiceProductDto dto);
        Task<ProductServiceProductDto?> UpdateProductAsync(int id, UpdateProductServiceProductDto dto);
        Task<bool> DeleteProductAsync(int id);
    }
}
