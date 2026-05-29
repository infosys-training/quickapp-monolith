using QuickApp.Core.Services.Shop.HttpClients;

namespace QuickApp.Core.Services.Shop
{
    public class OrdersService(IOrderServiceClient orderServiceClient) : IOrdersService
    {
        public Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            return orderServiceClient.GetAllOrdersAsync();
        }

        public Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            return orderServiceClient.GetOrderByIdAsync(id);
        }

        public Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return orderServiceClient.GetOrdersByCustomerIdAsync(customerId);
        }

        public Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            return orderServiceClient.CreateOrderAsync(dto);
        }

        public Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            return orderServiceClient.UpdateOrderAsync(id, dto);
        }

        public Task<bool> DeleteOrderAsync(int id)
        {
            return orderServiceClient.DeleteOrderAsync(id);
        }
    }
}
