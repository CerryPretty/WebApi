using WebApi.Models; 

namespace WebUI.Models
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ClientDisplayName { get; set; } = string.Empty;
        public string ManagerDisplayName { get; set; } = string.Empty;
        public string MasterDisplayName { get; set; } = string.Empty;
        public string ProblemDescription { get; set; } = string.Empty;
        public OrderStatus? Status { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? Cost { get; set; }
        public string? MasterComments { get; set; } 

        public ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
    }
}
