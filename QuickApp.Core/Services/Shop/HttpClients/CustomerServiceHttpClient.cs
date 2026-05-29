using System.Net.Http.Json;

namespace QuickApp.Core.Services.Shop.HttpClients;

public class CustomerServiceHttpClient : ICustomerServiceClient
{
    private readonly HttpClient _httpClient;

    public CustomerServiceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CustomerServiceDto>> GetAllCustomersAsync()
    {
        var response = await _httpClient.GetAsync("api/customer");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CustomerServiceDto>>() ?? [];
    }

    public async Task<CustomerServiceDto?> GetCustomerByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/customer/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CustomerServiceDto>();
    }

    public async Task<CustomerServiceDto> CreateCustomerAsync(CreateCustomerServiceDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/customer", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CustomerServiceDto>()
            ?? throw new InvalidOperationException("Failed to deserialize created customer.");
    }

    public async Task<CustomerServiceDto?> UpdateCustomerAsync(int id, UpdateCustomerServiceDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/customer/{id}", dto);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CustomerServiceDto>();
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/customer/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return false;

        response.EnsureSuccessStatusCode();
        return true;
    }
}
