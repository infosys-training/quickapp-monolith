using MobileShopBilling.Application.DTOs;

namespace MobileShopBilling.Application.Interfaces;

public interface ISupplierService
{
    Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
    Task<SupplierDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<SupplierDto>> GetAllAsync();
    Task UpdateAsync(UpdateSupplierDto dto);
    Task DeleteAsync(Guid id);
}
