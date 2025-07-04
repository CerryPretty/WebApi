using WebApi.Intarface;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;


namespace WebApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders
                           .Include(o => o.Status)
                           .OrderByDescending(o => o.CreatedDate)
                           .ToList();
        }

        public Order? GetOrderById(int orderId)
        {
            return _context.Orders
                           .Include(o => o.Status)
                           .FirstOrDefault(o => o.Id == orderId);
        }

        public IEnumerable<Order> GetOrdersByStatusId(int statusId)
        {
            return _context.Orders
                           .Include(o => o.Status)
                           .Where(o => o.StatusId == statusId)
                           .OrderByDescending(o => o.CreatedDate)
                           .ToList();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public IQueryable<Order> GetAll()
        {
            return _context.Orders
                           .Include(o => o.Status)
                           .Include(o => o.OrderServices)
                               .ThenInclude(os => os.Service)
                           .AsQueryable();
        }

        public Order Get(int id)
        {
            return _context.Orders
                           .Include(o => o.Status)
                           .Include(o => o.OrderServices)
                               .ThenInclude(os => os.Service)
                           .Include(o => o.OrderLogs)
                           .FirstOrDefault(o => o.Id == id);
        }

        public void Create(Order item)
        {
            _context.Orders.Add(item);
            _context.SaveChanges();
        }

        public void Update(Order item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        public async Task CreateOrderWithServices(Order order, IEnumerable<WebApi.Models.OrderService> orderServices)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();

                    foreach (var os in orderServices)
                    {
                        os.OrderId = order.Id;
                        await _context.OrderServices.AddAsync(os);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error creating order with services.", ex);
                }
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByClientId(string clientId)
        {
            return await _context.Orders
                                 .Where(o => o.ClientId == clientId)
                                 .Include(o => o.OrderServices)
                                     .ThenInclude(os => os.Service)
                                 .Include(o => o.Status)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByClientIdAsync(string clientId)
        {
            return await _context.Orders
                                 .Include(o => o.Status)
                                 .Where(o => o.ClientId == clientId)
                                 .ToListAsync();
        }

        public IQueryable<Order> GetOrdersByManagerId(string managerId)
        {
            return _context.Orders
                           .Include(o => o.Status)
                           .Where(o => o.ManagerId == managerId)
                           .AsQueryable();
        }

        public IQueryable<Order> GetOrdersByMasterId(string masterId)
        {
            return _context.Orders
                .Include(o => o.OrderServices)
                    .ThenInclude(os => os.Service)
                .Include(o => o.Status)
                .Where(o => o.MasterId == masterId);
        }

        public Order GetOrderByOrderNumber(string orderNumber)
        {
            return _context.Orders
                           .Include(o => o.Status)
                           .Include(o => o.OrderServices)
                               .ThenInclude(os => os.Service)
                           .FirstOrDefault(o => o.OrderNumber == orderNumber);
        }
    }
}