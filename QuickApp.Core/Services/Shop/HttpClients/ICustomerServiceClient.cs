namespace QuickApp.Core.Services.Shop.HttpClients;

public interface ICustomerServiceClient
{
    Task<IEnumerable<CustomerServiceDto>> GetAllCustomersAsync();
    Task<CustomerServiceDto?> GetCustomerByIdAsync(int id);
    Task<CustomerServiceDto> CreateCustomerAsync(CreateCustomerServiceDto dto);
    Task<CustomerServiceDto?> UpdateCustomerAsync(int id, UpdateCustomerServiceDto dto);
    Task<bool> DeleteCustomerAsync(int id);
}
