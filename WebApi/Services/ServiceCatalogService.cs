using WebApi.Interface;
using WebApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApi.Data;
using System.Threading.Tasks;
using WebApi.Intarface;

namespace WebApi.Services
{
    public class ServiceCatalogService : IServiceCatalogService
    {
        private readonly AppDbContext _context;

        public ServiceCatalogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceCatalog>> GetAllServiceCatalogs()
        {
            return await _context.ServiceCatalogs.ToListAsync();
        }

        public IQueryable<ServiceCatalog> GetAll()
        {
            return _context.ServiceCatalogs;
        }

        public ServiceCatalog Get(int id)
        {
            return _context.ServiceCatalogs.Find(id);
        }

        // *** Added this method in the previous turn, so it's included here. ***
        public async Task<ServiceCatalog> GetServiceCatalogById(int id)
        {
            return await _context.ServiceCatalogs.FirstOrDefaultAsync(s => s.Id == id);
        }

        public void Create(ServiceCatalog item)
        {
            _context.ServiceCatalogs.Add(item);
            _context.SaveChanges();
        }

        public void Update(ServiceCatalog item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var serviceCatalog = _context.ServiceCatalogs.Find(id);
            if (serviceCatalog != null)
            {
                _context.ServiceCatalogs.Remove(serviceCatalog);
                _context.SaveChanges();
            }
        }


    }
}