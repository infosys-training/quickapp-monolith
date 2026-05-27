namespace MobileShopBilling.Application.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GstNumber { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GstNumber { get; set; }
}

public class UpdateCustomerDto : CreateCustomerDto
{
    public Guid Id { get; set; }
}
