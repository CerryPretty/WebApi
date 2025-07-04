// In WebApi.Intarface/IOrderServiceService.cs
using WebApi.Models;
using System.Linq;
using System.Threading.Tasks; // For async operations

namespace WebApi.Intarface
{
    // This interface specifically for the OrderService (the join entity)
    public interface IOrderServiceService : IService<OrderService>
    {
       
    }
}