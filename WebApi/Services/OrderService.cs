using WebApi.Intarface;
using WebApi.Models; // Keep this using for WebApi.Models.Order
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        // --- Методы, требуемые OrderManagerController (синхронные) ---
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
                               .ThenInclude(os => os.Service) // To load associated services
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

        // --- FIXED METHOD: Explicitly qualify WebApi.Models.OrderService ---
        public async Task CreateOrderWithServices(Order order, IEnumerable<WebApi.Models.OrderService> orderServices)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add the main order first
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync(); // Save to get the Order.Id

                    // Assign the newly generated OrderId to each WebApi.Models.OrderService entry
                    foreach (var os in orderServices) // 'os' is correctly inferred as WebApi.Models.OrderService now
                    {
                        os.OrderId = order.Id;
                        await _context.OrderServices.AddAsync(os);
                    }

                    await _context.SaveChangesAsync(); // Save the OrderService entries
                    await transaction.CommitAsync(); // Commit the transaction if all is successful
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); // Rollback if any error occurs
                    // Log the exception if you have a logger here
                    throw new Exception("Error creating order with services.", ex); // Re-throw with a more specific message
                }
            }
        }
        // --- END FIXED METHOD ---

        public async Task<IEnumerable<Order>> GetOrdersByClientId(string clientId)
        {
            return await _context.Orders
                                 .Where(o => o.ClientId == clientId)
                                 .Include(o => o.OrderServices) // Include related order services
                                     .ThenInclude(os => os.Service) // Then include the actual service details
                                 .Include(o => o.Status)       // Include the order status
                                      // Include manager details
                                 .ToListAsync(); // <-- THIS IS THE KEY CHANGE
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

        // Пример реализации GetOrdersByMasterId в вашем OrderService (не интерфейс!)
        public IQueryable<Order> GetOrdersByMasterId(string masterId)
        {
            // Eager Loading: Подгружаем OrderServices и ServiceCatalog (через Service)
            // А также менеджера и клиента (ApplicationUser) и статус заказа.
            return _context.Orders
                .Include(o => o.OrderServices)
                    .ThenInclude(os => os.Service) // <-- Подгружаем ServiceCatalog через свойство 'Service'
                .Include(o => o.Status) // Подгружаем статус заказа
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