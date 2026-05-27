using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;

namespace MobileShopBilling.Web.Controllers;

[Authorize(Roles = "ShopAdmin,Staff")]
public class BillingController : Controller
{
    private readonly IInvoiceService _invoiceService;
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;
    private readonly ITaxConfigurationService _taxService;

    public BillingController(
        IInvoiceService invoiceService,
        IProductService productService,
        ICustomerService customerService,
        ITaxConfigurationService taxService)
    {
        _invoiceService = invoiceService;
        _productService = productService;
        _customerService = customerService;
        _taxService = taxService;
    }

    public async Task<IActionResult> Index()
    {
        var invoices = await _invoiceService.GetAllAsync();
        return View(invoices);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Products = await _productService.GetAllAsync();
        ViewBag.Customers = await _customerService.GetAllAsync();
        ViewBag.DefaultTax = await _taxService.GetDefaultAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
    {
        try
        {
            var invoice = await _invoiceService.CreateAsync(dto);
            return Json(new { success = true, id = invoice.Id, invoiceNumber = invoice.InvoiceNumber });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice == null) return NotFound();
        return View(invoice);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadPdf(Guid id)
    {
        var pdf = await _invoiceService.GeneratePdfAsync(id);
        var invoice = await _invoiceService.GetByIdAsync(id);
        return File(pdf, "application/pdf", $"{invoice?.InvoiceNumber ?? "invoice"}.pdf");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _invoiceService.CancelAsync(id);
        TempData["Success"] = "Invoice cancelled successfully.";
        return RedirectToAction(nameof(Index));
    }
}
