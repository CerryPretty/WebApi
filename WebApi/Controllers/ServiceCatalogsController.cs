using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebApi.Intarface;
using Microsoft.EntityFrameworkCore; 
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")] 
    [ApiController] 
    public class ServiceCatalogsController : ControllerBase
    {
        private readonly IServiceCatalogService _serviceCatalogService;

        public ServiceCatalogsController(IServiceCatalogService serviceCatalogService)
        {
            _serviceCatalogService = serviceCatalogService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ServiceCatalog>> GetServiceCatalogs()
        {
            var serviceCatalogs = _serviceCatalogService.GetAll();
            return Ok(serviceCatalogs.ToList()); 
        }

        [HttpGet("{id}")]
        public ActionResult<ServiceCatalog> GetServiceCatalog(int id)
        {
            var serviceCatalog = _serviceCatalogService.Get(id);

            if (serviceCatalog == null)
            {
                return NotFound(); 
            }

            return Ok(serviceCatalog); 
        }

        [HttpPost]
        public ActionResult<ServiceCatalog> CreateServiceCatalog([FromBody] ServiceCatalog serviceCatalog)
        {
            if (serviceCatalog == null)
            {
                return BadRequest(); 
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            _serviceCatalogService.Create(serviceCatalog);

            return CreatedAtAction(nameof(GetServiceCatalog), new { id = serviceCatalog.Id }, serviceCatalog);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateServiceCatalog(int id, [FromBody] ServiceCatalog serviceCatalog)
        {
            if (serviceCatalog == null || id != serviceCatalog.Id)
            {
                return BadRequest(); 
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
               
                if (_serviceCatalogService.Get(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw; 
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteServiceCatalog(int id)
        {
            var serviceCatalog = _serviceCatalogService.Get(id);
            if (serviceCatalog == null)
            {
                return NotFound(); 
            }

            _serviceCatalogService.Delete(id);
            return NoContent(); 
        }
    }
}