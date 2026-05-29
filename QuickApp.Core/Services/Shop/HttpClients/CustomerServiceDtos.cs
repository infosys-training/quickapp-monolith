namespace QuickApp.Core.Services.Shop.HttpClients;

public record CustomerServiceDto(
    int Id,
    string Name,
    string Email,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? Gender
);

public record CreateCustomerServiceDto(
    string Name,
    string Email,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? Gender
);

public record UpdateCustomerServiceDto(
    string Name,
    string Email,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? Gender
);
