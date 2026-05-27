using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;
using MobileShopBilling.Domain.Entities;
using MobileShopBilling.Domain.Interfaces;

namespace MobileShopBilling.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IRepository<Customer> _repository;

    public CustomerService(IRepository<Customer> repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            GstNumber = dto.GstNumber
        };

        await _repository.AddAsync(customer);
        return MapToDto(customer);
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _repository.GetAllAsync();
        return customers.Select(MapToDto);
    }

    public async Task UpdateAsync(UpdateCustomerDto dto)
    {
        var customer = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Customer not found");

        customer.Name = dto.Name;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;
        customer.Address = dto.Address;
        customer.GstNumber = dto.GstNumber;

        await _repository.UpdateAsync(customer);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<CustomerDto>> SearchAsync(string term)
    {
        var lower = term.ToLower();
        var customers = await _repository.FindAsync(c =>
            c.Name.ToLower().Contains(lower) ||
            c.Phone.Contains(term) ||
            (c.Email != null && c.Email.ToLower().Contains(lower)));
        return customers.Select(MapToDto);
    }

    private static CustomerDto MapToDto(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone,
        Address = c.Address,
        GstNumber = c.GstNumber,
        IsActive = c.IsActive
    };
}
