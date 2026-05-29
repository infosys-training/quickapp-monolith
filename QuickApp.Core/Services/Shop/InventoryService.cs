using QuickApp.Core.Services.Shop.HttpClients;

namespace QuickApp.Core.Services.Shop;

public class InventoryService : IInventoryService
{
    private readonly IInventoryServiceClient _client;

    public InventoryService(IInventoryServiceClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<InventoryItemDto>> GetAllAsync()
    {
        return await _client.GetAllAsync();
    }

    public async Task<InventoryItemDto?> GetByIdAsync(int id)
    {
        return await _client.GetByIdAsync(id);
    }

    public async Task<InventoryItemDto> CreateAsync(CreateInventoryItemDto dto)
    {
        return await _client.CreateAsync(dto);
    }

    public async Task<InventoryItemDto?> UpdateAsync(int id, UpdateInventoryItemDto dto)
    {
        return await _client.UpdateAsync(id, dto);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _client.DeleteAsync(id);
    }
}
