using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace QuickApp.Core.Services.Shop.HttpClients;

public class OrderServiceHttpClient : IOrderServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderServiceHttpClient> _logger;

    public OrderServiceHttpClient(HttpClient httpClient, ILogger<OrderServiceHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        try
        {
            var orders = await _httpClient.GetFromJsonAsync<IEnumerable<OrderDto>>("api/order");
            return orders ?? [];
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get orders from order-service");
            return [];
        }
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<OrderDto>($"api/order/{id}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get order {OrderId} from order-service", id);
            return null;
        }
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
    {
        try
        {
            var orders = await _httpClient.GetFromJsonAsync<IEnumerable<OrderDto>>(
                $"api/order/customer/{customerId}");
            return orders ?? [];
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get orders for customer {CustomerId} from order-service", customerId);
            return [];
        }
    }

    public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/order", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to create order via order-service");
            return null;
        }
    }

    public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/order/{id}", dto);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to update order {OrderId} via order-service", id);
            return null;
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/order/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to delete order {OrderId} via order-service", id);
            return false;
        }
    }
}
