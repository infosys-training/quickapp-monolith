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
        [ProducesResponseType(typeof(IEnumerable<OrderVM>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderServiceClient.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderServiceClient.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderVM>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var orders = await _orderServiceClient.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderVM), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> Create([FromBody] CreateOrderVM dto)
        {
            var order = await _orderServiceClient.CreateOrderAsync(dto);
            if (order == null)
                return StatusCode(StatusCodes.Status502BadGateway, "Failed to create order in Order service");
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OrderVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderVM dto)
        {
            var result = await _orderServiceClient.UpdateOrderAsync(id, dto);

            if (result.IsSuccess)
                return Ok(result.Data);

            if (result.IsNotFound)
                return NotFound();

            return StatusCode((int?)result.StatusCode ?? StatusCodes.Status502BadGateway, result.Error);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _orderServiceClient.DeleteOrderAsync(id);

            if (result.IsSuccess)
                return NoContent();

            if (result.IsNotFound)
                return NotFound();

            return StatusCode((int?)result.StatusCode ?? StatusCodes.Status502BadGateway, result.Error);
        }
    }
}
