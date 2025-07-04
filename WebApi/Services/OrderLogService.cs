using WebApi.Interface;
using WebApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using System;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public class OrderLogService : IOrderLogService
    {
        private readonly AppDbContext _context;

        public OrderLogService(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<OrderLog> GetAll()
        {
            return _context.OrderLogs.AsQueryable();
        }

        public OrderLog Get(int id)
        {
            return _context.OrderLogs.Find(id);
        }

        public void Create(OrderLog item)
        {
            _context.OrderLogs.Add(item);
            _context.SaveChanges();
        }

        public void Update(OrderLog item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var orderLog = _context.OrderLogs.Find(id);
            if (orderLog != null)
            {
                _context.OrderLogs.Remove(orderLog);
                _context.SaveChanges();
            }
        }

        public IQueryable<OrderLog> GetLogsByOrderId(int orderId)
        {
            return _context.OrderLogs.Where(ol => ol.OrderId == orderId).AsQueryable();
        }

        public IQueryable<OrderLog> GetLogsByUserId(string userId)
        {
            return _context.OrderLogs.Where(ol => ol.UserId == userId).AsQueryable();
        }

        public IQueryable<OrderLog> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.OrderLogs
                           .Where(ol => ol.EventDate >= startDate && ol.EventDate <= endDate)
                           .AsQueryable();
        }
    }
}