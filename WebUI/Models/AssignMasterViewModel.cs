using Microsoft.AspNetCore.Mvc.Rendering; 
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class AssignMasterViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, выберите заказ.")]
        [Display(Name = "Выберите Заказ")]
        public int SelectedOrderId { get; set; } 

        [Required(ErrorMessage = "Пожалуйста, выберите мастера.")]
        [Display(Name = "Выберите Мастера")]
        public string SelectedMasterId { get; set; } 

        [Required(ErrorMessage = "Стоимость обязательна.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Стоимость должна быть положительным числом.")] 
        [Display(Name = "Стоимость Заказа")]
        public decimal Cost { get; set; }

        [StringLength(500, ErrorMessage = "Комментарии менеджера не могут превышать 500 символов.")]
        [Display(Name = "Комментарии Менеджера")]
        public string? ManagerComments { get; set; }

        public List<SelectListItem> Orders { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Masters { get; set; } = new List<SelectListItem>();
    }
}