namespace QuickApp.Core.Services.Shop.HttpClients;

public interface IInventoryServiceClient
{
    Task<IReadOnlyList<InventoryItemDto>> GetAllAsync();
    Task<InventoryItemDto?> GetByIdAsync(int id);
    Task<InventoryItemDto> CreateAsync(CreateInventoryItemDto dto);
    Task<InventoryItemDto?> UpdateAsync(int id, UpdateInventoryItemDto dto);
    Task<bool> DeleteAsync(int id);
}
