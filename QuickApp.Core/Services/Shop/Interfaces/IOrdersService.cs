using QuickApp.Core.Services.Shop.Contracts;

namespace QuickApp.Core.Services.Shop;

public interface IOrdersService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDto?> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderDto?> UpdateOrderAsync(int id, CreateOrderRequest request);
    Task<bool> DeleteOrderAsync(int id);
}
