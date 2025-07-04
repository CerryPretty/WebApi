// WebUI/Models/MasterAssignedTasksViewModel.cs
using System.Collections.Generic;

namespace WebUI.Models
{
    public class MasterAssignedTasksViewModel
    {
        public IEnumerable<OrderViewModel> AssignedOrders { get; set; } = new List<OrderViewModel>();
        public string MasterDisplayName { get; set; } = string.Empty;
    }
}