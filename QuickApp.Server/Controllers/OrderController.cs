using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickApp.Server.Services;
using QuickApp.Server.ViewModels.Shop;

namespace QuickApp.Server.Controllers
{
    [Authorize]
    public class OrderController : BaseApiController
    {
        private readonly IOrderServiceClient _orderServiceClient;

        public OrderController(ILogger<BaseApiController> logger, IMapper mapper,
            IOrderServiceClient orderServiceClient)
            : base(logger, mapper)
        {
            _orderServiceClient = orderServiceClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderServiceClient.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderServiceClient.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var orders = await _orderServiceClient.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderVM dto)
        {
            var order = await _orderServiceClient.CreateOrderAsync(dto);
            if (order == null)
                return StatusCode(502, "Failed to create order in Order service");
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] object dto)
        {
            var order = await _orderServiceClient.UpdateOrderAsync(id, dto);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _orderServiceClient.DeleteOrderAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
