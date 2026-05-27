namespace MobileShopBilling.Application.DTOs;

public class TaxConfigurationDto
{
    public Guid Id { get; set; }
    public string TaxName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public class CreateTaxConfigurationDto
{
    public string TaxName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsDefault { get; set; }
}

public class UpdateTaxConfigurationDto : CreateTaxConfigurationDto
{
    public Guid Id { get; set; }
}
