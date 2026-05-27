using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;
using MobileShopBilling.Domain.Entities;
using MobileShopBilling.Domain.Interfaces;

namespace MobileShopBilling.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IRepository<Supplier> _repository;

    public SupplierService(IRepository<Supplier> repository)
    {
        _repository = repository;
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
    {
        var supplier = new Supplier
        {
            Name = dto.Name,
            ContactPerson = dto.ContactPerson,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            GstNumber = dto.GstNumber
        };

        await _repository.AddAsync(supplier);
        return MapToDto(supplier);
    }

    public async Task<SupplierDto?> GetByIdAsync(Guid id)
    {
        var supplier = await _repository.GetByIdAsync(id);
        return supplier == null ? null : MapToDto(supplier);
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync()
    {
        var suppliers = await _repository.GetAllAsync();
        return suppliers.Select(MapToDto);
    }

    public async Task UpdateAsync(UpdateSupplierDto dto)
    {
        var supplier = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Supplier not found");

        supplier.Name = dto.Name;
        supplier.ContactPerson = dto.ContactPerson;
        supplier.Email = dto.Email;
        supplier.Phone = dto.Phone;
        supplier.Address = dto.Address;
        supplier.GstNumber = dto.GstNumber;

        await _repository.UpdateAsync(supplier);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    private static SupplierDto MapToDto(Supplier s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        ContactPerson = s.ContactPerson,
        Email = s.Email,
        Phone = s.Phone,
        Address = s.Address,
        GstNumber = s.GstNumber,
        IsActive = s.IsActive
    };
}
