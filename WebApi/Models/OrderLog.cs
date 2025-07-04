using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class OrderLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime EventDate { get; set; } = DateTime.Now;

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [StringLength(5)]
        public string OrderNumber { get; set; }

        [StringLength(100)]
        public string ItemCode { get; set; } 

        [StringLength(200)]
        public string ItemName { get; set; } 

        [StringLength(200)]
        public string CustomerName { get; set; } 

        [Required]
        [StringLength(1000)]
        public string EventDescription { get; set; }

     
        [StringLength(450)]
        public string UserId { get; set; }

        [StringLength(256)]
        public string UserDisplayName { get; set; }
    }
}