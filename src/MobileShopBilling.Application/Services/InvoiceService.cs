using Microsoft.EntityFrameworkCore;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;
using MobileShopBilling.Domain.Entities;
using MobileShopBilling.Domain.Enums;
using MobileShopBilling.Domain.Interfaces;

namespace MobileShopBilling.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IRepository<Invoice> _invoiceRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Customer> _customerRepository;

    public InvoiceService(
        IRepository<Invoice> invoiceRepository,
        IRepository<Product> productRepository,
        IRepository<Customer> customerRepository)
    {
        _invoiceRepository = invoiceRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
    }

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto)
    {
        var invoiceNumber = await GenerateInvoiceNumber();

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = dto.CustomerId,
            Notes = dto.Notes,
            Status = InvoiceStatus.Completed
        };

        decimal subTotal = 0;
        decimal totalTax = 0;
        decimal totalDiscount = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId)
                ?? throw new InvalidOperationException($"Product {item.ProductId} not found");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}. Available: {product.StockQuantity}");

            var lineTotal = item.UnitPrice * item.Quantity;
            var discountAmount = lineTotal * (item.DiscountPercent / 100);
            var taxableAmount = lineTotal - discountAmount;
            var taxAmount = taxableAmount * (item.TaxPercent / 100);
            var totalPrice = taxableAmount + taxAmount;

            invoice.Items.Add(new InvoiceItem
            {
                ProductId = item.ProductId,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercent = item.DiscountPercent,
                DiscountAmount = discountAmount,
                TaxPercent = item.TaxPercent,
                TaxAmount = taxAmount,
                TotalPrice = totalPrice
            });

            subTotal += lineTotal;
            totalTax += taxAmount;
            totalDiscount += discountAmount;

            product.StockQuantity -= item.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        invoice.SubTotal = subTotal;
        invoice.TaxAmount = totalTax;
        invoice.DiscountAmount = totalDiscount;
        invoice.TotalAmount = subTotal - totalDiscount + totalTax;

        invoice.Payment = new Payment
        {
            Amount = invoice.TotalAmount,
            PaymentMode = dto.PaymentMode,
            TransactionReference = dto.TransactionReference,
            PaymentDate = DateTime.UtcNow
        };

        await _invoiceRepository.AddAsync(invoice);
        return MapToDto(invoice);
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var invoice = await _invoiceRepository.Query()
            .Include(i => i.Items)
            .ThenInclude(item => item.Product)
            .Include(i => i.Customer)
            .Include(i => i.Payment)
            .FirstOrDefaultAsync(i => i.Id == id);

        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
    {
        var invoices = await _invoiceRepository.Query()
            .Include(i => i.Customer)
            .Include(i => i.Payment)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();

        return invoices.Select(MapToDto);
    }

    public async Task<IEnumerable<InvoiceDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var invoices = await _invoiceRepository.Query()
            .Include(i => i.Customer)
            .Include(i => i.Payment)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();

        return invoices.Select(MapToDto);
    }

    public async Task CancelAsync(Guid id)
    {
        var invoice = await _invoiceRepository.Query()
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new InvalidOperationException("Invoice not found");

        foreach (var item in invoice.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        invoice.Status = InvoiceStatus.Cancelled;
        await _invoiceRepository.UpdateAsync(invoice);
    }

    public async Task<byte[]> GeneratePdfAsync(Guid id)
    {
        var invoice = await _invoiceRepository.Query()
            .Include(i => i.Items)
            .Include(i => i.Customer)
            .Include(i => i.Payment)
            .Include(i => i.Tenant)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new InvalidOperationException("Invoice not found");

        return InvoicePdfGenerator.Generate(invoice);
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var products = _productRepository.Query();
        var invoices = _invoiceRepository.Query()
            .Include(i => i.Customer)
            .Include(i => i.Items);

        var totalProducts = await products.CountAsync();
        var lowStockProducts = await products.CountAsync(p => p.StockQuantity <= p.LowStockThreshold);
        var totalInvoices = await invoices.CountAsync(i => i.Status == InvoiceStatus.Completed);
        var totalRevenue = await invoices
            .Where(i => i.Status == InvoiceStatus.Completed)
            .SumAsync(i => i.TotalAmount);
        var todayInvoices = await invoices
            .Where(i => i.InvoiceDate.Date == today && i.Status == InvoiceStatus.Completed)
            .CountAsync();
        var todayRevenue = await invoices
            .Where(i => i.InvoiceDate.Date == today && i.Status == InvoiceStatus.Completed)
            .SumAsync(i => i.TotalAmount);

        var totalCustomers = await _customerRepository.CountAsync();

        var recentInvoices = await invoices
            .OrderByDescending(i => i.InvoiceDate)
            .Take(10)
            .Select(i => new RecentInvoiceDto
            {
                InvoiceNumber = i.InvoiceNumber,
                CustomerName = i.Customer != null ? i.Customer.Name : "Walk-in",
                TotalAmount = i.TotalAmount,
                InvoiceDate = i.InvoiceDate
            })
            .ToListAsync();

        var thirtyDaysAgo = today.AddDays(-30);
        var topProducts = await invoices
            .Where(i => i.Status == InvoiceStatus.Completed && i.InvoiceDate >= thirtyDaysAgo)
            .SelectMany(i => i.Items)
            .GroupBy(item => new { item.ProductId, item.ProductName })
            .Select(g => new TopProductDto
            {
                ProductName = g.Key.ProductName,
                QuantitySold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(p => p.Revenue)
            .Take(5)
            .ToListAsync();

        return new DashboardDto
        {
            TotalProducts = totalProducts,
            TotalCustomers = totalCustomers,
            TotalInvoices = totalInvoices,
            TotalRevenue = totalRevenue,
            TodayRevenue = todayRevenue,
            TodayInvoices = todayInvoices,
            LowStockProducts = lowStockProducts,
            RecentInvoices = recentInvoices,
            TopProducts = topProducts
        };
    }

    public async Task<IEnumerable<SalesReportDto>> GetSalesReportAsync(DateTime from, DateTime to)
    {
        return await _invoiceRepository.Query()
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.Status == InvoiceStatus.Completed)
            .GroupBy(i => i.InvoiceDate.Date)
            .Select(g => new SalesReportDto
            {
                Date = g.Key,
                InvoiceCount = g.Count(),
                TotalAmount = g.Sum(i => i.TotalAmount),
                TaxAmount = g.Sum(i => i.TaxAmount),
                DiscountAmount = g.Sum(i => i.DiscountAmount)
            })
            .OrderBy(r => r.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductSalesReportDto>> GetProductSalesReportAsync(DateTime from, DateTime to)
    {
        return await _invoiceRepository.Query()
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.Status == InvoiceStatus.Completed)
            .SelectMany(i => i.Items)
            .GroupBy(item => new { item.ProductId, item.ProductName })
            .Select(g => new ProductSalesReportDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                QuantitySold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(r => r.Revenue)
            .ToListAsync();
    }

    private async Task<string> GenerateInvoiceNumber()
    {
        var today = DateTime.UtcNow;
        var prefix = $"INV-{today:yyyyMMdd}";
        var count = await _invoiceRepository.CountAsync(i => i.InvoiceNumber.StartsWith(prefix));
        return $"{prefix}-{(count + 1):D4}";
    }

    private static InvoiceDto MapToDto(Invoice i) => new()
    {
        Id = i.Id,
        InvoiceNumber = i.InvoiceNumber,
        InvoiceDate = i.InvoiceDate,
        CustomerId = i.CustomerId,
        CustomerName = i.Customer?.Name,
        SubTotal = i.SubTotal,
        TaxAmount = i.TaxAmount,
        DiscountAmount = i.DiscountAmount,
        TotalAmount = i.TotalAmount,
        Status = i.Status,
        Notes = i.Notes,
        PaymentMode = i.Payment?.PaymentMode,
        TransactionReference = i.Payment?.TransactionReference,
        Items = i.Items.Select(item => new InvoiceItemDto
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            DiscountPercent = item.DiscountPercent,
            DiscountAmount = item.DiscountAmount,
            TaxPercent = item.TaxPercent,
            TaxAmount = item.TaxAmount,
            TotalPrice = item.TotalPrice
        }).ToList()
    };
}
