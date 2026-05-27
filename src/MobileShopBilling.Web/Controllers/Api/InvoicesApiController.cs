using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;

namespace MobileShopBilling.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ShopAdmin,Staff")]
public class InvoicesApiController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesApiController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
    {
        return Ok(await _invoiceService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetById(Guid id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        return invoice == null ? NotFound() : Ok(invoice);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create([FromBody] CreateInvoiceDto dto)
    {
        var invoice = await _invoiceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> DownloadPdf(Guid id)
    {
        var pdf = await _invoiceService.GeneratePdfAsync(id);
        return File(pdf, "application/pdf");
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _invoiceService.CancelAsync(id);
        return NoContent();
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        return Ok(await _invoiceService.GetDashboardAsync());
    }

    [HttpGet("reports/sales")]
    public async Task<ActionResult<IEnumerable<SalesReportDto>>> GetSalesReport(
        [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        return Ok(await _invoiceService.GetSalesReportAsync(from, to));
    }

    [HttpGet("reports/products")]
    public async Task<ActionResult<IEnumerable<ProductSalesReportDto>>> GetProductSalesReport(
        [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        return Ok(await _invoiceService.GetProductSalesReportAsync(from, to));
    }
}
