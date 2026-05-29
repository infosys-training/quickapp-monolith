// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using QuickApp.Core.Services.Shop.HttpClients;

namespace QuickApp.Core.Services.Shop
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerServiceDto>> GetAllCustomersDataAsync();
        Task<CustomerServiceDto?> GetCustomerByIdAsync(int id);
    }
}
