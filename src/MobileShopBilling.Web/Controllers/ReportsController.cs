using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.Interfaces;

namespace MobileShopBilling.Web.Controllers;

[Authorize(Roles = "ShopAdmin,Staff")]
public class ReportsController : Controller
{
    private readonly IInvoiceService _invoiceService;

    public ReportsController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Sales(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow.Date.AddDays(1);

        ViewBag.From = fromDate;
        ViewBag.To = toDate;

        var report = await _invoiceService.GetSalesReportAsync(fromDate, toDate);
        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> ProductSales(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow.Date.AddDays(1);

        ViewBag.From = fromDate;
        ViewBag.To = toDate;

        var report = await _invoiceService.GetProductSalesReportAsync(fromDate, toDate);
        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> InvoiceHistory(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow.Date.AddDays(1);

        ViewBag.From = fromDate;
        ViewBag.To = toDate;

        var invoices = await _invoiceService.GetByDateRangeAsync(fromDate, toDate);
        return View(invoices);
    }

    [HttpGet]
    public async Task<IActionResult> ExportSales(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow.Date.AddDays(1);

        var report = await _invoiceService.GetSalesReportAsync(fromDate, toDate);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sales Report");

        worksheet.Cell(1, 1).Value = "Date";
        worksheet.Cell(1, 2).Value = "Invoices";
        worksheet.Cell(1, 3).Value = "Total Amount";
        worksheet.Cell(1, 4).Value = "Tax Amount";
        worksheet.Cell(1, 5).Value = "Discount Amount";

        var headerRange = worksheet.Range(1, 1, 1, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
        headerRange.Style.Font.FontColor = XLColor.White;

        var row = 2;
        foreach (var item in report)
        {
            worksheet.Cell(row, 1).Value = item.Date.ToString("yyyy-MM-dd");
            worksheet.Cell(row, 2).Value = item.InvoiceCount;
            worksheet.Cell(row, 3).Value = item.TotalAmount;
            worksheet.Cell(row, 4).Value = item.TaxAmount;
            worksheet.Cell(row, 5).Value = item.DiscountAmount;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"SalesReport_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportProductSales(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow.Date.AddDays(1);

        var report = await _invoiceService.GetProductSalesReportAsync(fromDate, toDate);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Product Sales");

        worksheet.Cell(1, 1).Value = "Product";
        worksheet.Cell(1, 2).Value = "Brand";
        worksheet.Cell(1, 3).Value = "Qty Sold";
        worksheet.Cell(1, 4).Value = "Revenue";

        var headerRange = worksheet.Range(1, 1, 1, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
        headerRange.Style.Font.FontColor = XLColor.White;

        var row = 2;
        foreach (var item in report)
        {
            worksheet.Cell(row, 1).Value = item.ProductName;
            worksheet.Cell(row, 2).Value = item.Brand ?? "";
            worksheet.Cell(row, 3).Value = item.QuantitySold;
            worksheet.Cell(row, 4).Value = item.Revenue;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"ProductSalesReport_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx");
    }
}
