using MobileShopBilling.Application.DTOs;

namespace MobileShopBilling.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateAsync(CreateUserDto dto, Guid tenantId);
    Task<IEnumerable<UserDto>> GetAllAsync(Guid tenantId);
    Task<UserDto?> GetByIdAsync(string id);
    Task UpdateAsync(UpdateUserDto dto);
    Task DeleteAsync(string id);
}
