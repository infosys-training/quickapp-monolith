namespace MobileShopBilling.Application.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class DashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public int TodayInvoices { get; set; }
    public int LowStockProducts { get; set; }
    public List<RecentInvoiceDto> RecentInvoices { get; set; } = new();
    public List<TopProductDto> TopProducts { get; set; } = new();
}

public class RecentInvoiceDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
}

public class TopProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class SuperAdminDashboardDto
{
    public int TotalShops { get; set; }
    public int ActiveShops { get; set; }
    public int InactiveShops { get; set; }
    public int TotalUsers { get; set; }
    public decimal TotalSystemRevenue { get; set; }
    public List<TenantDto> RecentShops { get; set; } = new();
}

public class SalesReportDto
{
    public DateTime Date { get; set; }
    public int InvoiceCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
}

public class ProductSalesReportDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}
