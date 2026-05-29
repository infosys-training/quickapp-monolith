namespace QuickApp.Core.Services.Shop.HttpClients;

public interface IOrderServiceClient
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId);
    Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto);
    Task<bool> DeleteOrderAsync(int id);
}
