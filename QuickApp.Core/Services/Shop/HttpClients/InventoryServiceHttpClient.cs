using System.Net.Http.Json;

namespace QuickApp.Core.Services.Shop.HttpClients;

public class InventoryServiceHttpClient : IInventoryServiceClient
{
    private readonly HttpClient _httpClient;

    public InventoryServiceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<InventoryItemDto>> GetAllAsync()
    {
        var items = await _httpClient.GetFromJsonAsync<List<InventoryItemDto>>("api/inventory");
        return items ?? [];
    }

    public async Task<InventoryItemDto?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/inventory/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<InventoryItemDto>();
    }

    public async Task<InventoryItemDto> CreateAsync(CreateInventoryItemDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/inventory", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<InventoryItemDto>())!;
    }

    public async Task<InventoryItemDto?> UpdateAsync(int id, UpdateInventoryItemDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/inventory/{id}", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<InventoryItemDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/inventory/{id}");
        return response.IsSuccessStatusCode;
    }
}
