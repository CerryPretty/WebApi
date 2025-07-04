// WebUI/Models/OrderManagerOrdersViewModel.cs
using System.Collections.Generic;
// using WebApi.Models; // Эту директиву можно убрать, если она не нужна здесь напрямую

namespace WebUI.Models
{
    public class OrderManagerOrdersViewModel
    {
        // Теперь эта коллекция содержит OrderViewModel
        public IEnumerable<OrderViewModel> AllOrders { get; set; } = new List<OrderViewModel>();

        public Dictionary<string, int> OrdersByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> OrdersCreatedMonthly { get; set; } = new Dictionary<string, int>();
    }
}