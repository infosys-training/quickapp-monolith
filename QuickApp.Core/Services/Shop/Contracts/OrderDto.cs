namespace QuickApp.Core.Services.Shop.Contracts;

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

public record CreateOrderRequest(
    decimal Discount,
    string? Comments,
    string? CashierId,
    int CustomerId,
    List<CreateOrderDetailRequest> OrderDetails
);

public record CreateOrderDetailRequest(
    decimal UnitPrice,
    int Quantity,
    decimal Discount,
    int ProductId
);
