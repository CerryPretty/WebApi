using WebApi.Intarface;
using WebApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly AppDbContext _context;

        public OrderStatusService(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<OrderStatus> GetAll()
        {
            return _context.OrderStatuses.AsQueryable();
        }

        public OrderStatus Get(int id)
        {
            return _context.OrderStatuses.Find(id);
        }

        public void Create(OrderStatus item)
        {
            _context.OrderStatuses.Add(item);
            _context.SaveChanges();
        }

        public void Update(OrderStatus item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var orderStatus = _context.OrderStatuses.Find(id);
            if (orderStatus != null)
            {
                _context.OrderStatuses.Remove(orderStatus);
                _context.SaveChanges();
            }
        }

        public OrderStatus GetStatusByName(string statusName)
        {
            return _context.OrderStatuses.FirstOrDefault(s => s.StatusName == statusName);
        }
    }
}