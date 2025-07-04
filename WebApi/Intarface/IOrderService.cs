using System;
using System.Linq;
using WebApi.Intarface;
using WebApi.Models;

namespace WebApi.Intarface
{
    public interface IOrderService : IService<Order>
    {

        // Методы, которые ваш OrderManagerController использовал (синхронные, как было в последних итерациях):
        IEnumerable<Order> GetAllOrders();
        Order? GetOrderById(int orderId);
        IEnumerable<Order> GetOrdersByStatusId(int statusId);
        void UpdateOrder(Order order);
        Task CreateOrderWithServices(Order order, IEnumerable<OrderService> orderServices);

        // Example: Get orders by ClientId
        Task<IEnumerable<Order>> GetOrdersByClientId(string clientId);

        // Example: Get orders by ManagerId
        IQueryable<Order> GetOrdersByManagerId(string managerId);

        // Example: Get orders by MasterId
        IQueryable<Order> GetOrdersByMasterId(string masterId);

        // Example: Get an order by its OrderNumber
        Order GetOrderByOrderNumber(string orderNumber);
    }
}