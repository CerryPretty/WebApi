using System; 
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class ServiceCatalog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

       
        [StringLength(2000)]
        public string? Description { get; set; } 

        [StringLength(500)]
        public string? ImageUrl { get; set; } 

      
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<OrderService> OrderServices { get; set; }
    }
}