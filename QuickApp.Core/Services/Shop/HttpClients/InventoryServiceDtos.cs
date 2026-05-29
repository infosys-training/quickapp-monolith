namespace QuickApp.Core.Services.Shop.HttpClients;

public record InventoryItemDto(
    int Id,
    int ProductId,
    int UnitsInStock,
    int ReorderLevel,
    bool IsActive,
    bool IsDiscontinued,
    decimal BuyingPrice,
    DateTime CreatedDate,
    DateTime UpdatedDate
);

public record CreateInventoryItemDto(
    int ProductId,
    int UnitsInStock,
    int ReorderLevel,
    bool IsActive,
    bool IsDiscontinued,
    decimal BuyingPrice
);

public record UpdateInventoryItemDto(
    int ProductId,
    int UnitsInStock,
    int ReorderLevel,
    bool IsActive,
    bool IsDiscontinued,
    decimal BuyingPrice
);
