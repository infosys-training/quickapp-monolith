namespace QuickApp.Core.Services.Shop
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId);
        Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto);
    }

    public record OrderDto(
        int Id,
        decimal Discount,
        string? Comments,
        string? CashierId,
        int CustomerId,
        DateTime CreatedDate,
        DateTime UpdatedDate,
        List<OrderDetailDto> OrderDetails
    );

    public record OrderDetailDto(
        int Id,
        decimal UnitPrice,
        int Quantity,
        decimal Discount,
        int ProductId,
        int OrderId
    );

    public record CreateOrderDto(
        decimal Discount,
        string? Comments,
        string? CashierId,
        int CustomerId,
        List<CreateOrderDetailDto> OrderDetails
    );

    public record CreateOrderDetailDto(
        decimal UnitPrice,
        int Quantity,
        decimal Discount,
        int ProductId
    );
}
