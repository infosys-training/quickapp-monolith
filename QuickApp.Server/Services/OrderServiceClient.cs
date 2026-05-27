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
        Task<ServiceResult<IEnumerable<OrderVM>>> GetAllOrdersAsync();
        Task<ServiceResult<OrderVM>> GetOrderByIdAsync(int id);
        Task<ServiceResult<IEnumerable<OrderVM>>> GetOrdersByCustomerIdAsync(int customerId);
        Task<ServiceResult<OrderVM>> CreateOrderAsync(CreateOrderVM dto);
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

        public async Task<ServiceResult<IEnumerable<OrderVM>>> GetAllOrdersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/order");

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Order service returned {StatusCode} for GetAll: {Body}",
                        response.StatusCode, body);
                    return new ServiceResult<IEnumerable<OrderVM>>(null, response.StatusCode, body);
                }

                var orders = await response.Content.ReadFromJsonAsync<IEnumerable<OrderVM>>();
                return new ServiceResult<IEnumerable<OrderVM>>(orders ?? [], response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders from Order service");
                return new ServiceResult<IEnumerable<OrderVM>>(null, null, ex.Message);
            }
        }

        public async Task<ServiceResult<OrderVM>> GetOrderByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/order/{id}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new ServiceResult<OrderVM>(null, HttpStatusCode.NotFound, null);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Order service returned {StatusCode} for GetById {OrderId}: {Body}",
                        response.StatusCode, id, body);
                    return new ServiceResult<OrderVM>(null, response.StatusCode, body);
                }

                var order = await response.Content.ReadFromJsonAsync<OrderVM>();
                return new ServiceResult<OrderVM>(order, response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order {OrderId} from Order service", id);
                return new ServiceResult<OrderVM>(null, null, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<OrderVM>>> GetOrdersByCustomerIdAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/order/customer/{customerId}");

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Order service returned {StatusCode} for customer {CustomerId}: {Body}",
                        response.StatusCode, customerId, body);
                    return new ServiceResult<IEnumerable<OrderVM>>(null, response.StatusCode, body);
                }

                var orders = await response.Content.ReadFromJsonAsync<IEnumerable<OrderVM>>();
                return new ServiceResult<IEnumerable<OrderVM>>(orders ?? [], response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders for customer {CustomerId} from Order service", customerId);
                return new ServiceResult<IEnumerable<OrderVM>>(null, null, ex.Message);
            }
        }

        public async Task<ServiceResult<OrderVM>> CreateOrderAsync(CreateOrderVM dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/order", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Order service returned {StatusCode} for Create: {Body}",
                        response.StatusCode, body);
                    return new ServiceResult<OrderVM>(null, response.StatusCode, body);
                }

                var order = await response.Content.ReadFromJsonAsync<OrderVM>();
                return new ServiceResult<OrderVM>(order, response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order in Order service");
                return new ServiceResult<OrderVM>(null, null, ex.Message);
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
