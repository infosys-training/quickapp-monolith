using QuickApp.Core.Services.Shop.HttpClients;

namespace QuickApp.Core.Services.Shop
{
    public class ProductService(IProductServiceClient productServiceClient) : IProductService
    {
        public Task<IList<ProductServiceProductDto>> GetAllProductsAsync()
        {
            return productServiceClient.GetAllProductsAsync();
        }

        public Task<ProductServiceProductDto?> GetProductByIdAsync(int id)
        {
            return productServiceClient.GetProductByIdAsync(id);
        }

        public Task<ProductServiceProductDto> CreateProductAsync(CreateProductServiceProductDto dto)
        {
            return productServiceClient.CreateProductAsync(dto);
        }

        public Task<ProductServiceProductDto?> UpdateProductAsync(int id, UpdateProductServiceProductDto dto)
        {
            return productServiceClient.UpdateProductAsync(id, dto);
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            return productServiceClient.DeleteProductAsync(id);
        }
    }
}
