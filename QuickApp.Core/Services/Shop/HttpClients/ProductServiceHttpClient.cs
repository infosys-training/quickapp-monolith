using System.Net;
using System.Net.Http.Json;

namespace QuickApp.Core.Services.Shop.HttpClients
{
    public class ProductServiceHttpClient(HttpClient httpClient) : IProductServiceClient
    {
        public async Task<IList<ProductServiceProductDto>> GetAllProductsAsync()
        {
            var result = await httpClient.GetFromJsonAsync<IList<ProductServiceProductDto>>("api/product");
            return result ?? [];
        }

        public async Task<ProductServiceProductDto?> GetProductByIdAsync(int id)
        {
            var response = await httpClient.GetAsync($"api/product/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductServiceProductDto>();
        }

        public async Task<ProductServiceProductDto> CreateProductAsync(CreateProductServiceProductDto dto)
        {
            var response = await httpClient.PostAsJsonAsync("api/product", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<ProductServiceProductDto>())!;
        }

        public async Task<ProductServiceProductDto?> UpdateProductAsync(int id, UpdateProductServiceProductDto dto)
        {
            var response = await httpClient.PutAsJsonAsync($"api/product/{id}", dto);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductServiceProductDto>();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"api/product/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<IList<ProductServiceCategoryDto>> GetAllCategoriesAsync()
        {
            var result = await httpClient.GetFromJsonAsync<IList<ProductServiceCategoryDto>>("api/product/categories");
            return result ?? [];
        }

        public async Task<ProductServiceCategoryDto?> GetCategoryByIdAsync(int id)
        {
            var response = await httpClient.GetAsync($"api/product/categories/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductServiceCategoryDto>();
        }

        public async Task<ProductServiceCategoryDto> CreateCategoryAsync(CreateProductServiceCategoryDto dto)
        {
            var response = await httpClient.PostAsJsonAsync("api/product/categories", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<ProductServiceCategoryDto>())!;
        }

        public async Task<ProductServiceCategoryDto?> UpdateCategoryAsync(int id, UpdateProductServiceCategoryDto dto)
        {
            var response = await httpClient.PutAsJsonAsync($"api/product/categories/{id}", dto);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductServiceCategoryDto>();
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"api/product/categories/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
