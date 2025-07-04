using Microsoft.AspNetCore.Mvc;
using WebApi.Intarface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderServicesController : ControllerBase
    {
        private readonly IOrderServiceService _orderServiceService;

        public OrderServicesController(IOrderServiceService orderServiceService)
        {
            _orderServiceService = orderServiceService;
        }

        [HttpGet]
        public IActionResult GetAllOrderServices()
        {
            var orderServices = _orderServiceService.GetAll();
            if (orderServices == null || !orderServices.Any())
            {
                return NotFound("No order services found.");
            }
            return Ok(orderServices);
        }
    }
}