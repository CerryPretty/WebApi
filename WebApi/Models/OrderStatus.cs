using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StatusName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}