using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;
using MobileShopBilling.Domain.Entities;

namespace MobileShopBilling.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, Guid tenantId)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            TenantId = tenantId,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        await _userManager.AddToRoleAsync(user, dto.Role);

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles.FirstOrDefault() ?? dto.Role);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(Guid tenantId)
    {
        var users = await _userManager.Users
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();

        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(MapToDto(user, roles.FirstOrDefault() ?? ""));
        }
        return result;
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles.FirstOrDefault() ?? "");
    }

    public async Task UpdateAsync(UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("User not found");

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, dto.Role);

        await _userManager.UpdateAsync(user);
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new InvalidOperationException("User not found");

        user.IsActive = false;
        await _userManager.UpdateAsync(user);
    }

    private static UserDto MapToDto(ApplicationUser u, string role) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email ?? "",
        PhoneNumber = u.PhoneNumber,
        Role = role,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
