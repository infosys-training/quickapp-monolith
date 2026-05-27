using MobileShopBilling.Application.DTOs;

namespace MobileShopBilling.Application.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto);
    Task<InvoiceDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<InvoiceDto>> GetAllAsync();
    Task<IEnumerable<InvoiceDto>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task CancelAsync(Guid id);
    Task<byte[]> GeneratePdfAsync(Guid id);
    Task<DashboardDto> GetDashboardAsync();
    Task<IEnumerable<SalesReportDto>> GetSalesReportAsync(DateTime from, DateTime to);
    Task<IEnumerable<ProductSalesReportDto>> GetProductSalesReportAsync(DateTime from, DateTime to);
}
