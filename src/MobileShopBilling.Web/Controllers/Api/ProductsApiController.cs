using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopBilling.Application.DTOs;
using MobileShopBilling.Application.Interfaces;

namespace MobileShopBilling.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ShopAdmin,Staff")]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsApiController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        return Ok(await _productService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Search([FromQuery] string term)
    {
        return Ok(await _productService.SearchAsync(term));
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStock()
    {
        return Ok(await _productService.GetLowStockProductsAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        dto.Id = id;
        await _productService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
