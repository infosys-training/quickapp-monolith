using System.Net;
using System.Net.Http.Json;
using QuickApp.Server.ViewModels.Shop;

namespace QuickApp.Server.Services
{
    public record ServiceResult<T>(T? Data, HttpStatusCode? StatusCode, string? Error)
    {
        public bool IsSuccess => StatusCode is >= HttpStatusCode.OK and < HttpStatusCode.MultipleChoices;
        public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;
    }

    public interface IOrderServiceClient
    {
        Task<IEnumerable<OrderVM>> GetAllOrdersAsync();
        Task<OrderVM?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderVM>> GetOrdersByCustomerIdAsync(int customerId);
        Task<OrderVM?> CreateOrderAsync(CreateOrderVM dto);
        Task<ServiceResult<OrderVM>> UpdateOrderAsync(int id, UpdateOrderVM dto);
        Task<ServiceResult<bool>> DeleteOrderAsync(int id);
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
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
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

        public async Task<ServiceResult<OrderVM>> UpdateOrderAsync(int id, UpdateOrderVM dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/order/{id}", dto);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new ServiceResult<OrderVM>(null, HttpStatusCode.NotFound, null);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Order service returned {StatusCode} for update of order {OrderId}: {Body}",
                        response.StatusCode, id, body);
                    return new ServiceResult<OrderVM>(null, response.StatusCode, body);
                }

                var order = await response.Content.ReadFromJsonAsync<OrderVM>();
                return new ServiceResult<OrderVM>(order, response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order {OrderId} in Order service", id);
                return new ServiceResult<OrderVM>(null, null, ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> DeleteOrderAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/order/{id}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new ServiceResult<bool>(false, HttpStatusCode.NotFound, null);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Order service returned {StatusCode} for delete of order {OrderId}: {Body}",
                        response.StatusCode, id, body);
                    return new ServiceResult<bool>(false, response.StatusCode, body);
                }

                return new ServiceResult<bool>(true, response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete order {OrderId} from Order service", id);
                return new ServiceResult<bool>(false, null, ex.Message);
            }
        }
    }
}
