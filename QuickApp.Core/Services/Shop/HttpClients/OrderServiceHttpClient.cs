using System.Net;
using System.Net.Http.Json;

namespace QuickApp.Core.Services.Shop.HttpClients
{
    public class OrderServiceHttpClient : IOrderServiceClient
    {
        private readonly HttpClient _httpClient;

        public OrderServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<OrderDto>>("api/order");
            return result ?? [];
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/order/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<OrderDto>>(
                $"api/order/customer/{customerId}");
            return result ?? [];
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/order", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<OrderDto>())!;
        }

        public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/order/{id}", dto);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/order/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
