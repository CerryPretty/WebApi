using System;
using System.Linq;
using WebApi.Intarface;
using WebApi.Models;

namespace WebApi.Interface
{
    public interface IOrderLogService : IService<OrderLog>
    {
       
        IQueryable<OrderLog> GetLogsByOrderId(int orderId);

       
        IQueryable<OrderLog> GetLogsByUserId(string userId);

        
        IQueryable<OrderLog> GetLogsByDateRange(DateTime startDate, DateTime endDate);
    }
}