using MobileShopBilling.Application.DTOs;

namespace MobileShopBilling.Application.Interfaces;

public interface ITaxConfigurationService
{
    Task<TaxConfigurationDto> CreateAsync(CreateTaxConfigurationDto dto);
    Task<TaxConfigurationDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaxConfigurationDto>> GetAllAsync();
    Task UpdateAsync(UpdateTaxConfigurationDto dto);
    Task DeleteAsync(Guid id);
    Task<TaxConfigurationDto?> GetDefaultAsync();
}
