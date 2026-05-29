namespace QuickApp.Core.Services.Shop.HttpClients
{
    public interface IProductServiceClient
    {
        Task<IList<ProductServiceProductDto>> GetAllProductsAsync();
        Task<ProductServiceProductDto?> GetProductByIdAsync(int id);
        Task<ProductServiceProductDto> CreateProductAsync(CreateProductServiceProductDto dto);
        Task<ProductServiceProductDto?> UpdateProductAsync(int id, UpdateProductServiceProductDto dto);
        Task<bool> DeleteProductAsync(int id);

        Task<IList<ProductServiceCategoryDto>> GetAllCategoriesAsync();
        Task<ProductServiceCategoryDto?> GetCategoryByIdAsync(int id);
        Task<ProductServiceCategoryDto> CreateCategoryAsync(CreateProductServiceCategoryDto dto);
        Task<ProductServiceCategoryDto?> UpdateCategoryAsync(int id, UpdateProductServiceCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
