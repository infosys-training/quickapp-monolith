using Microsoft.AspNetCore.Mvc;
using QuickApp.Core.Services.Shop;

namespace QuickApp.Server.Controllers
{
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _ordersService.GetOrderByIdAsync(id);
            if (order is null) return NotFound();
            return Ok(order);
        }

        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var orders = await _ordersService.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            _logger.LogInformation("Creating order for customer {CustomerId} via order-service", dto.CustomerId);
            var order = await _ordersService.CreateOrderAsync(dto);
            if (order is null) return BadRequest();
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
    }
}
