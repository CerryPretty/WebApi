
using WebApi.Models;

namespace WebApi.Intarface
{
    public interface IOrderService : IService<Order>
    {
        IEnumerable<Order> GetAllOrders();
        Order? GetOrderById(int orderId);
        IEnumerable<Order> GetOrdersByStatusId(int statusId);
        void UpdateOrder(Order order);
        Task CreateOrderWithServices(Order order, IEnumerable<OrderService> orderServices);
        Task<IEnumerable<Order>> GetOrdersByClientId(string clientId);
        IQueryable<Order> GetOrdersByManagerId(string managerId);
        IQueryable<Order> GetOrdersByMasterId(string masterId);
        Order GetOrderByOrderNumber(string orderNumber);
    }
}