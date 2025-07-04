using Microsoft.AspNetCore.Mvc;
using WebApi.Intarface;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusesController : ControllerBase
    {
        private readonly IOrderStatusService _orderStatusService;

        public OrderStatusesController(IOrderStatusService orderStatusService)
        {
            _orderStatusService = orderStatusService;
        }

        [HttpGet]
        public IActionResult GetAllOrderStatuses()
        {
            var statuses = _orderStatusService.GetAll();
            if (statuses == null || !statuses.Any())
            {
                return NotFound("No order statuses found.");
            }
            return Ok(statuses);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderStatusById(int id)
        {
            var status = _orderStatusService.Get(id);
            if (status == null)
            {
                return NotFound($"Order status with ID {id} not found.");
            }
            return Ok(status);
        }

        [HttpGet("byname/{statusName}")]
        public IActionResult GetOrderStatusByName(string statusName)
        {
            var status = _orderStatusService.GetStatusByName(statusName);
            if (status == null)
            {
                return NotFound($"Order status with name '{statusName}' not found.");
            }
            return Ok(status);
        }

        [HttpPost]
        public IActionResult CreateOrderStatus([FromBody] OrderStatus orderStatus)
        {
            if (orderStatus == null)
            {
                return BadRequest("Order status data is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _orderStatusService.Create(orderStatus);
            return CreatedAtAction(nameof(GetOrderStatusById), new { id = orderStatus.Id }, orderStatus);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrderStatus(int id, [FromBody] OrderStatus orderStatus)
        {
            if (orderStatus == null || orderStatus.Id != id)
            {
                return BadRequest("Order status data is invalid or ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingStatus = _orderStatusService.Get(id);
            if (existingStatus == null)
            {
                return NotFound($"Order status with ID {id} not found.");
            }

            _orderStatusService.Update(orderStatus);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrderStatus(int id)
        {
            var statusToDelete = _orderStatusService.Get(id);
            if (statusToDelete == null)
            {
                return NotFound($"Order status with ID {id} not found.");
            }

            _orderStatusService.Delete(id);
            return NoContent();
        }
    }
}