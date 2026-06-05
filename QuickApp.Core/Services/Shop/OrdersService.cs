using System.Net.Http.Json;

namespace QuickApp.Core.Services.Shop
{
    public class OrdersService : IOrdersService
    {
        private readonly HttpClient _httpClient;

        public OrdersService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<OrderDto>>("api/order");
            return result ?? [];
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/order/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var result = await _httpClient.GetFromJsonAsync<List<OrderDto>>($"api/order/customer/{customerId}");
            return result ?? [];
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/order", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        }
    }
}
