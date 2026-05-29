namespace QuickApp.Core.Services.Shop.HttpClients
{
    public record ProductServiceProductDto(
        int Id,
        string Name,
        string? Description,
        string? Icon,
        decimal BuyingPrice,
        decimal SellingPrice,
        int UnitsInStock,
        bool IsActive,
        bool IsDiscontinued,
        int ProductCategoryId,
        string? ProductCategoryName,
        int? ParentId);

    public record CreateProductServiceProductDto(
        string Name,
        string? Description,
        string? Icon,
        decimal BuyingPrice,
        decimal SellingPrice,
        int UnitsInStock,
        bool IsActive,
        bool IsDiscontinued,
        int ProductCategoryId,
        int? ParentId);

    public record UpdateProductServiceProductDto(
        string Name,
        string? Description,
        string? Icon,
        decimal BuyingPrice,
        decimal SellingPrice,
        int UnitsInStock,
        bool IsActive,
        bool IsDiscontinued,
        int ProductCategoryId,
        int? ParentId);

    public record ProductServiceCategoryDto(
        int Id,
        string Name,
        string? Description,
        string? Icon);

    public record CreateProductServiceCategoryDto(
        string Name,
        string? Description,
        string? Icon);

    public record UpdateProductServiceCategoryDto(
        string Name,
        string? Description,
        string? Icon);
}
