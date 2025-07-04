
using WebApi.Models;

namespace WebApi.Intarface
{
    public interface IOrderStatusService : IService<OrderStatus>
    {
        OrderStatus GetStatusByName(string statusName);
    }
}