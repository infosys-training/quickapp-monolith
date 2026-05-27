using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;
using MobileShopBilling.Domain.Entities;
using MobileShopBilling.Domain.Interfaces;

namespace MobileShopBilling.Application.Services;

public class TaxConfigurationService : ITaxConfigurationService
{
    private readonly IRepository<TaxConfiguration> _repository;

    public TaxConfigurationService(IRepository<TaxConfiguration> repository)
    {
        _repository = repository;
    }

    public async Task<TaxConfigurationDto> CreateAsync(CreateTaxConfigurationDto dto)
    {
        if (dto.IsDefault)
        {
            var existing = await _repository.FindAsync(t => t.IsDefault);
            foreach (var t in existing)
            {
                t.IsDefault = false;
                await _repository.UpdateAsync(t);
            }
        }

        var tax = new TaxConfiguration
        {
            TaxName = dto.TaxName,
            Rate = dto.Rate,
            IsDefault = dto.IsDefault
        };

        await _repository.AddAsync(tax);
        return MapToDto(tax);
    }

    public async Task<TaxConfigurationDto?> GetByIdAsync(Guid id)
    {
        var tax = await _repository.GetByIdAsync(id);
        return tax == null ? null : MapToDto(tax);
    }

    public async Task<IEnumerable<TaxConfigurationDto>> GetAllAsync()
    {
        var taxes = await _repository.GetAllAsync();
        return taxes.Select(MapToDto);
    }

    public async Task UpdateAsync(UpdateTaxConfigurationDto dto)
    {
        var tax = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Tax configuration not found");

        if (dto.IsDefault)
        {
            var existing = await _repository.FindAsync(t => t.IsDefault && t.Id != dto.Id);
            foreach (var t in existing)
            {
                t.IsDefault = false;
                await _repository.UpdateAsync(t);
            }
        }

        tax.TaxName = dto.TaxName;
        tax.Rate = dto.Rate;
        tax.IsDefault = dto.IsDefault;

        await _repository.UpdateAsync(tax);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<TaxConfigurationDto?> GetDefaultAsync()
    {
        var taxes = await _repository.FindAsync(t => t.IsDefault && t.IsActive);
        var tax = taxes.FirstOrDefault();
        return tax == null ? null : MapToDto(tax);
    }

    private static TaxConfigurationDto MapToDto(TaxConfiguration t) => new()
    {
        Id = t.Id,
        TaxName = t.TaxName,
        Rate = t.Rate,
        IsDefault = t.IsDefault,
        IsActive = t.IsActive
    };
}
