using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderLogsController : ControllerBase
    {
        private readonly IOrderLogService _orderLogService;

        public OrderLogsController(IOrderLogService orderLogService)
        {
            _orderLogService = orderLogService;
        }

        [HttpGet]
        public IActionResult GetAllOrderLogs()
        {
            var orderLogs = _orderLogService.GetAll();
            if (orderLogs == null || !orderLogs.Any())
            {
                return NotFound("No order logs found.");
            }
            return Ok(orderLogs);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderLogById(int id)
        {
            var orderLog = _orderLogService.Get(id);
            if (orderLog == null)
            {
                return NotFound($"Order log with ID {id} not found.");
            }
            return Ok(orderLog);
        }

        [HttpPost]
        public IActionResult CreateOrderLog([FromBody] OrderLog orderLog)
        {
            if (orderLog == null)
            {
                return BadRequest("Order log data is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _orderLogService.Create(orderLog);
            return CreatedAtAction(nameof(GetOrderLogById), new { id = orderLog.Id }, orderLog);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrderLog(int id, [FromBody] OrderLog orderLog)
        {
            if (orderLog == null || orderLog.Id != id)
            {
                return BadRequest("Order log data is invalid or ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrderLog = _orderLogService.Get(id);
            if (existingOrderLog == null)
            {
                return NotFound($"Order log with ID {id} not found.");
            }

            _orderLogService.Update(orderLog);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrderLog(int id)
        {
            var orderLogToDelete = _orderLogService.Get(id);
            if (orderLogToDelete == null)
            {
                return NotFound($"Order log with ID {id} not found.");
            }

            _orderLogService.Delete(id);
            return NoContent();
        }

        [HttpGet("byorder/{orderId}")]
        public IActionResult GetOrderLogsByOrderId(int orderId)
        {
            var orderLogs = _orderLogService.GetLogsByOrderId(orderId);
            if (orderLogs == null || !orderLogs.Any())
            {
                return NotFound($"No order logs found for Order ID {orderId}.");
            }
            return Ok(orderLogs);
        }

        [HttpGet("byuser/{userId}")]
        public IActionResult GetOrderLogsByUserId(string userId)
        {
            var orderLogs = _orderLogService.GetLogsByUserId(userId);
            if (orderLogs == null || !orderLogs.Any())
            {
                return NotFound($"No order logs found for User ID {userId}.");
            }
            return Ok(orderLogs);
        }

        [HttpGet("bydaterange")]
        public IActionResult GetOrderLogsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest("Start date and end date are required for this query.");
            }

            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be after end date.");
            }

            var orderLogs = _orderLogService.GetLogsByDateRange(startDate, endDate);
            if (orderLogs == null || !orderLogs.Any())
            {
                return NotFound($"No order logs found between {startDate.ToShortDateString()} and {endDate.ToShortDateString()}.");
            }
            return Ok(orderLogs);
        }
    }
}