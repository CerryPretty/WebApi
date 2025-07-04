using Microsoft.EntityFrameworkCore;
using WebApi.Data; 
using WebApi.Intarface;

namespace WebApi.Services
{
    public class OrderServiceService : IOrderServiceService
    {
        private readonly AppDbContext _context;

        public OrderServiceService(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<WebApi.Models.OrderService> GetAll()
        {
            return _context.OrderServices.AsQueryable();
        }

        public WebApi.Models.OrderService Get(int id)
        {
            throw new NotImplementedException("");
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
            throw new NotImplementedException("");
        }

        public IQueryable<WebApi.Models.OrderService> GetServicesForOrder(int orderId)
        {
            return _context.OrderServices
                            .Where(os => os.OrderId == orderId)
                            .Include(os => os.Service) 
                            .AsQueryable();
        }

        public IQueryable<WebApi.Models.OrderService> GetOrdersIncludingService(int serviceId)
        {
            return _context.OrderServices
                            .Where(os => os.ServiceId == serviceId)
                            .Include(os => os.Order) 
                            .AsQueryable();
        }

       
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
