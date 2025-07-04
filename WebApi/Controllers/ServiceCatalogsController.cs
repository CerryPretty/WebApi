using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebApi.Intarface;
using Microsoft.EntityFrameworkCore; 
using WebApi.Models;
using System.Threading.Tasks; // For async operations if you choose to make them async

namespace WebApi.Controllers
{
    [Route("api/[controller]")] // Defines the base route for the controller, e.g., /api/ServiceCatalogs
    [ApiController] // Indicates that this class is an API controller
    public class ServiceCatalogsController : ControllerBase
    {
        private readonly IServiceCatalogService _serviceCatalogService;

        public ServiceCatalogsController(IServiceCatalogService serviceCatalogService)
        {
            _serviceCatalogService = serviceCatalogService;
        }

        // GET: api/ServiceCatalogs
        [HttpGet]
        public ActionResult<IEnumerable<ServiceCatalog>> GetServiceCatalogs()
        {
            var serviceCatalogs = _serviceCatalogService.GetAll();
            return Ok(serviceCatalogs.ToList()); // .ToList() executes the query and returns a list
        }

        // GET: api/ServiceCatalogs/5
        [HttpGet("{id}")]
        public ActionResult<ServiceCatalog> GetServiceCatalog(int id)
        {
            var serviceCatalog = _serviceCatalogService.Get(id);

            if (serviceCatalog == null)
            {
                return NotFound(); // Returns 404 Not Found if the item doesn't exist
            }

            return Ok(serviceCatalog); // Returns 200 OK with the ServiceCatalog item
        }

        // POST: api/ServiceCatalogs
        [HttpPost]
        public ActionResult<ServiceCatalog> CreateServiceCatalog([FromBody] ServiceCatalog serviceCatalog)
        {
            if (serviceCatalog == null)
            {
                return BadRequest(); // Returns 400 Bad Request if the body is null
            }

            // You might want to add model validation here if not already handled by [ApiController]
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns 400 Bad Request with validation errors
            }

            _serviceCatalogService.Create(serviceCatalog);

            // Returns 201 Created status, a Location header with the URL of the new resource,
            // and the newly created object in the body.
            return CreatedAtAction(nameof(GetServiceCatalog), new { id = serviceCatalog.Id }, serviceCatalog);
        }

        // PUT: api/ServiceCatalogs/5
        [HttpPut("{id}")]
        public IActionResult UpdateServiceCatalog(int id, [FromBody] ServiceCatalog serviceCatalog)
        {
            if (serviceCatalog == null || id != serviceCatalog.Id)
            {
                return BadRequest(); // Returns 400 Bad Request if IDs don't match or body is null
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _serviceCatalogService.Update(serviceCatalog);
            }
            catch (DbUpdateConcurrencyException)
            {
                // This catch block is useful if you implement optimistic concurrency
                // For a simple CRUD, you might check if the item exists before attempting to update.
                if (_serviceCatalogService.Get(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw; // Re-throw other concurrency exceptions
                }
            }

            return NoContent(); // Returns 204 No Content for a successful update
        }

        // DELETE: api/ServiceCatalogs/5
        [HttpDelete("{id}")]
        public IActionResult DeleteServiceCatalog(int id)
        {
            var serviceCatalog = _serviceCatalogService.Get(id);
            if (serviceCatalog == null)
            {
                return NotFound(); // Returns 404 Not Found if the item doesn't exist
            }

            _serviceCatalogService.Delete(id);
            return NoContent(); // Returns 204 No Content for a successful deletion
        }
    }
}