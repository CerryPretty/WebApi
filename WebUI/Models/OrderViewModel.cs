
namespace WebUI.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string ServiceName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ClientDisplayName { get; set; } = string.Empty;
        public string ManagerDisplayName { get; set; } = string.Empty; 
        public string MasterDisplayName { get; set; } = string.Empty;   
        public string ProblemDescription { get; set; } = string.Empty;
        public decimal? Cost { get; set; }
        public string? ManagerComments { get; set; }

        public string? MasterComments { get; set; }

        public WebApi.Models.OrderStatus Status { get; set; } = null!;
    }
}