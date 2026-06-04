using System.Net.Http.Json;
using QuickApp.Core.Services.Shop.Contracts;

namespace QuickApp.Core.Services.Shop;

public class OrdersService : IOrdersService
{
    private readonly HttpClient _httpClient;

    public OrdersService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var response = await _httpClient.GetAsync("/api/order");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>() ?? [];
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/order/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }

    public async Task<OrderDto?> CreateOrderAsync(CreateOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/order", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }

    public async Task<OrderDto?> UpdateOrderAsync(int id, CreateOrderRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/order/{id}", request);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/order/{id}");
        return response.IsSuccessStatusCode;
    }
}
