// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using QuickApp.Core.Services.Shop.HttpClients;

namespace QuickApp.Core.Services.Shop
{
    public class CustomerService(ICustomerServiceClient customerServiceClient) : ICustomerService
    {
        public async Task<IEnumerable<CustomerServiceDto>> GetAllCustomersDataAsync()
        {
            return await customerServiceClient.GetAllCustomersAsync();
        }

        public async Task<CustomerServiceDto?> GetCustomerByIdAsync(int id)
        {
            return await customerServiceClient.GetCustomerByIdAsync(id);
        }
    }
}
