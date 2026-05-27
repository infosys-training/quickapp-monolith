using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;

namespace MobileShopBilling.Web.Controllers;

[Authorize(Roles = "ShopAdmin,Staff")]
public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();
        return View(products);
    }

    public async Task<IActionResult> LowStock()
    {
        var products = await _productService.GetLowStockProductsAsync();
        return View("Index", products);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _productService.CreateAsync(dto);
        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();

        var dto = new UpdateProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Brand = product.Brand,
            IMEI = product.IMEI,
            Barcode = product.Barcode,
            Description = product.Description,
            Category = product.Category,
            PurchasePrice = product.PurchasePrice,
            SellingPrice = product.SellingPrice,
            StockQuantity = product.StockQuantity,
            LowStockThreshold = product.LowStockThreshold
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _productService.UpdateAsync(dto);
        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        TempData["Success"] = "Product deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        var products = await _productService.SearchAsync(term ?? "");
        return Json(products);
    }
}
