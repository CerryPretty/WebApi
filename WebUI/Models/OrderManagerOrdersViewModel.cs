
namespace WebUI.Models
{
    public class OrderManagerOrdersViewModel
    {
        public IEnumerable<OrderViewModel> AllOrders { get; set; } = new List<OrderViewModel>();

        public Dictionary<string, int> OrdersByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> OrdersCreatedMonthly { get; set; } = new Dictionary<string, int>();
    }
}