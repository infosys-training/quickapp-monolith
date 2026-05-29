using QuickApp.Core.Services.Shop.HttpClients;

namespace QuickApp.Core.Services.Shop
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrderServiceClient _orderServiceClient;

        public OrdersService(IOrderServiceClient orderServiceClient)
        {
            _orderServiceClient = orderServiceClient;
        }

        public Task<IEnumerable<OrderDto>> GetAllOrdersAsync() =>
            _orderServiceClient.GetAllOrdersAsync();

        public Task<OrderDto?> GetOrderByIdAsync(int id) =>
            _orderServiceClient.GetOrderByIdAsync(id);

        public Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId) =>
            _orderServiceClient.GetOrdersByCustomerIdAsync(customerId);

        public Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto) =>
            _orderServiceClient.CreateOrderAsync(dto);

        public Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto) =>
            _orderServiceClient.UpdateOrderAsync(id, dto);

        public Task<bool> DeleteOrderAsync(int id) =>
            _orderServiceClient.DeleteOrderAsync(id);
    }
}
