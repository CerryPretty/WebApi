// WebUI.Models/ServiceCatalogViewModel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel; // For DisplayName attribute

namespace WebUI.Models
{
    public class ServiceCatalogViewModel
    {
        // Unique identifier for the service. Hidden from the user in most forms.
        public int Id { get; set; }

        [Required(ErrorMessage = "Название услуги обязательно.")]
        [StringLength(100, ErrorMessage = "Название услуги не может превышать 100 символов.")]
        [DisplayName("Название услуги")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Цена услуги обязательна.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Цена должна быть положительным числом.")]
        [DataType(DataType.Currency)]
        [DisplayName("Цена")]
        public decimal Price { get; set; }

        [StringLength(50, ErrorMessage = "Категория не может превышать 50 символов.")]
        [DisplayName("Категория")]
        public string Category { get; set; } // E.g., "Ремонт телефонов", "Ремонт ноутбуков"

        [StringLength(500, ErrorMessage = "Описание не может превышать 500 символов.")]
        [DisplayName("Описание")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("URL изображения")]
        public string ImageUrl { get; set; } // Optional: URL to an image representing the service
    }
}