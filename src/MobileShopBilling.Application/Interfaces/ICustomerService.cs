using MobileShopBilling.Application.DTOs;

namespace MobileShopBilling.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task UpdateAsync(UpdateCustomerDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<CustomerDto>> SearchAsync(string term);
}
