using System; // Добавить для DateTime
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

        // Новое поле: Подробное описание услуги
        [StringLength(2000)] // Увеличьте длину по необходимости
        public string? Description { get; set; } // Может быть null, если нет детального описания

        [StringLength(500)]
        public string? ImageUrl { get; set; } // URL изображения (может быть null)

        // Новые поля: Даты создания и последнего обновления
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Дата создания, обязательное, по умолчанию текущая
        public DateTime? UpdatedAt { get; set; } // Дата последнего обновления, может быть null

        public virtual ICollection<OrderService> OrderServices { get; set; }
    }
}