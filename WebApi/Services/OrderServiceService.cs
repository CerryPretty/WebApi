using WebApi.Interface;
using WebApi.Models; // This MUST be the namespace for your data models
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Data; // Assuming AppDbContext is here
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using WebApi.Intarface;
using WebApi.Services;

namespace WebApi.Services
{
    public class OrderServiceService : IOrderServiceService
    {
        private readonly AppDbContext _context;

        public OrderServiceService(AppDbContext context)
        {
            _context = context;
        }

        // Methods implementing IService<WebApi.Models.OrderService>
        // All references to the OrderService model (the join entity) are now fully qualified.
        public IQueryable<WebApi.Models.OrderService> GetAll()
        {
            return _context.OrderServices.AsQueryable();
        }

        public WebApi.Models.OrderService Get(int id)
        {
            throw new NotImplementedException("This method is not applicable for WebApi.Models.OrderService due to its composite key. Use a method that takes both orderId and serviceId, or query via GetAll() and filter.");
        }

        public void Create(WebApi.Models.OrderService item)
        {
            _context.OrderServices.Add(item);
            _context.SaveChanges();
        }

        public void Update(WebApi.Models.OrderService item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException("This method is not applicable for WebApi.Models.OrderService due to its composite key. Use a method that takes both orderId and serviceId to delete.");
        }

        // Methods implementing IOrderServiceService directly
        // These methods specifically return/take the WebApi.Models.OrderService type.
        public IQueryable<WebApi.Models.OrderService> GetServicesForOrder(int orderId)
        {
            return _context.OrderServices
                            .Where(os => os.OrderId == orderId)
                            .Include(os => os.Service) // Eagerly load the ServiceCatalog details
                            .AsQueryable();
        }

        public IQueryable<WebApi.Models.OrderService> GetOrdersIncludingService(int serviceId)
        {
            return _context.OrderServices
                            .Where(os => os.ServiceId == serviceId)
                            .Include(os => os.Order) // Eagerly load the Order details
                            .AsQueryable();
        }

        // Internal helper methods for composite key operations (not part of the interface contract)
        // These are helper methods specific to this class's implementation.
        internal WebApi.Models.OrderService GetByCompositeKey(int orderId, int serviceId)
        {
            return _context.OrderServices.Find(orderId, serviceId);
        }

        internal void DeleteByCompositeKey(int orderId, int serviceId)
        {
            var orderService = _context.OrderServices.Find(orderId, serviceId);
            if (orderService != null)
            {
                _context.OrderServices.Remove(orderService);
                _context.SaveChanges();
            }
        }
    }
}
