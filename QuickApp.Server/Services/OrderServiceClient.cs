using System.Net.Http.Json;
using QuickApp.Server.ViewModels.Shop;

namespace QuickApp.Server.Services
{
    public interface IOrderServiceClient
    {
        Task<IEnumerable<OrderVM>> GetAllOrdersAsync();
        Task<OrderVM?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderVM>> GetOrdersByCustomerIdAsync(int customerId);
        Task<OrderVM?> CreateOrderAsync(CreateOrderVM dto);
        Task<OrderVM?> UpdateOrderAsync(int id, object dto);
        Task<bool> DeleteOrderAsync(int id);
    }

    public class OrderServiceClient : IOrderServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderServiceClient> _logger;

        public OrderServiceClient(HttpClient httpClient, ILogger<OrderServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderVM>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _httpClient.GetFromJsonAsync<IEnumerable<OrderVM>>("api/order");
                return orders ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders from Order service");
                return [];
            }
        }

        public async Task<OrderVM?> GetOrderByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OrderVM>($"api/order/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order {OrderId} from Order service", id);
                return null;
            }
        }

        public async Task<IEnumerable<OrderVM>> GetOrdersByCustomerIdAsync(int customerId)
        {
            try
            {
                var orders = await _httpClient.GetFromJsonAsync<IEnumerable<OrderVM>>($"api/order/customer/{customerId}");
                return orders ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders for customer {CustomerId} from Order service", customerId);
                return [];
            }
        }

        public async Task<OrderVM?> CreateOrderAsync(CreateOrderVM dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/order", dto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<OrderVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order in Order service");
                return null;
            }
        }

        public async Task<OrderVM?> UpdateOrderAsync(int id, object dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/order/{id}", dto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<OrderVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order {OrderId} in Order service", id);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete order {OrderId} from Order service", id);
                return false;
            }
        }
    }
}
