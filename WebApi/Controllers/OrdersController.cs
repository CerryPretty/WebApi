using Microsoft.AspNetCore.Mvc;
using WebApi.Intarface;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrders();
            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found.");
            }
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(order);
        }

        [HttpGet("status/{statusId}")]
        public IActionResult GetOrdersByStatusId(int statusId)
        {
            var orders = _orderService.GetOrdersByStatusId(statusId);
            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for status ID {statusId}.");
            }
            return Ok(orders);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] Order order)
        {
            if (order == null || order.Id != id)
            {
                return BadRequest("Order data is invalid or ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrder = _orderService.GetOrderById(id);
            if (existingOrder == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            order.Id = id;
            _orderService.UpdateOrder(order);
            return NoContent();
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("Order data is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _orderService.Create(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var orderToDelete = _orderService.GetOrderById(id);
            if (orderToDelete == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            _orderService.Delete(id);
            return NoContent();
        }

        [HttpPost("with-services")]
        public async Task<IActionResult> CreateOrderWithServices([FromBody] OrderCreationRequest request)
        {
            if (request == null || request.Order == null || request.OrderServices == null)
            {
                return BadRequest("Invalid request data. Order and order services are required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _orderService.CreateOrderWithServices(request.Order, request.OrderServices);
                return CreatedAtAction(nameof(GetOrderById), new { id = request.Order.Id }, request.Order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("byclient/{clientId}")]
        public async Task<IActionResult> GetOrdersByClientId(string clientId)
        {
            var orders = await _orderService.GetOrdersByClientId(clientId);
            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for client ID {clientId}.");
            }
            return Ok(orders);
        }

        [HttpGet("bymanager/{managerId}")]
        public IActionResult GetOrdersByManagerId(string managerId)
        {
            var orders = _orderService.GetOrdersByManagerId(managerId);
            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for manager ID {managerId}.");
            }
            return Ok(orders);
        }

        [HttpGet("bymaster/{masterId}")]
        public IActionResult GetOrdersByMasterId(string masterId)
        {
            var orders = _orderService.GetOrdersByMasterId(masterId);
            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for master ID {masterId}.");
            }
            return Ok(orders);
        }

        [HttpGet("byordernumber/{orderNumber}")]
        public IActionResult GetOrderByOrderNumber(string orderNumber)
        {
            var order = _orderService.GetOrderByOrderNumber(orderNumber);
            if (order == null)
            {
                return NotFound($"Order with number {orderNumber} not found.");
            }
            return Ok(order);
        }
    }

    public class OrderCreationRequest
    {
        public Order Order { get; set; }
        public IEnumerable<WebApi.Models.OrderService> OrderServices { get; set; }
    }
}