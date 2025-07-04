using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string OrderNumber { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

      
        [Required]
        [StringLength(450)] 
        public string ClientId { get; set; }

        [StringLength(450)]
        public string ManagerId { get; set; }

        [StringLength(450)]
        public string? MasterId { get; set; } 

        [Required]
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual OrderStatus Status { get; set; }

        [Required]
        [StringLength(1000)]
        public string ProblemDescription { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Cost { get; set; }

        [StringLength(500)]
        public string? ManagerComments { get; set; } 

        [StringLength(500)]
        public string? MasterComments { get; set; }  

        public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
        public virtual ICollection<OrderLog> OrderLogs { get; set; } = new List<OrderLog>();

        [StringLength(256)]
        public string? ClientDisplayName { get; set; } 
    }
}