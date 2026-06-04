using Microsoft.AspNetCore.Mvc;
using QuickApp.Core.Services.Shop;
using QuickApp.Core.Services.Shop.Contracts;

namespace QuickApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrdersService _ordersService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrdersService ordersService, ILogger<OrderController> logger)
    {
        _ordersService = ordersService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _ordersService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _ordersService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var created = await _ordersService.CreateOrderAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created?.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrderRequest request)
    {
        var updated = await _ordersService.UpdateOrderAsync(id, request);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _ordersService.DeleteOrderAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
