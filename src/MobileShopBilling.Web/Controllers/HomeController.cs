using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.Interfaces;

namespace MobileShopBilling.Web.Controllers;

[Authorize(Roles = "ShopAdmin,Staff")]
public class HomeController : Controller
{
    private readonly IInvoiceService _invoiceService;
    private readonly IProductService _productService;

    public HomeController(IInvoiceService invoiceService, IProductService productService)
    {
        _invoiceService = invoiceService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboard = await _invoiceService.GetDashboardAsync();
        return View(dashboard);
    }
}
