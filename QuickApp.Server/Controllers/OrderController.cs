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

        public OrderController(ILogger<OrderController> logger, IMapper mapper,
            IOrderServiceClient orderServiceClient)
            : base(logger, mapper)
        {
            _orderServiceClient = orderServiceClient;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderServiceClient.GetAllOrdersAsync();

            if (result.IsSuccess)
                return Ok(result.Data);

            return StatusCode((int?)result.StatusCode ?? StatusCodes.Status502BadGateway, result.Error);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _orderServiceClient.GetOrderByIdAsync(id);

            if (result.IsSuccess)
                return Ok(result.Data);

            if (result.IsNotFound)
                return NotFound();

            return StatusCode((int?)result.StatusCode ?? StatusCodes.Status502BadGateway, result.Error);
        }

        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var result = await _orderServiceClient.GetOrdersByCustomerIdAsync(customerId);

            if (result.IsSuccess)
                return Ok(result.Data);

            return StatusCode((int?)result.StatusCode ?? StatusCodes.Status502BadGateway, result.Error);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderVM), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> Create([FromBody] CreateOrderVM dto)
        {
            var result = await _orderServiceClient.CreateOrderAsync(dto);

            if (result.IsSuccess && result.Data != null)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);

            if (result.IsSuccess)
                return StatusCode(StatusCodes.Status502BadGateway, "Order service returned success but no order data");

            return StatusCode((int?)result.StatusCode ?? StatusCodes.Status502BadGateway,
                result.Error ?? "Failed to create order in Order service");
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
