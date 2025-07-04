using Microsoft.AspNetCore.Mvc.Rendering; // Для SelectListItem
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class AssignMasterViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, выберите заказ.")]
        [Display(Name = "Выберите Заказ")]
        public int SelectedOrderId { get; set; } // ID выбранного заказа

        [Required(ErrorMessage = "Пожалуйста, выберите мастера.")]
        [Display(Name = "Выберите Мастера")]
        public string SelectedMasterId { get; set; } // ID выбранного мастера (string, так как IdentityUser ID - string)

        // Новое поле для стоимости заказа
        [Required(ErrorMessage = "Стоимость обязательна.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Стоимость должна быть положительным числом.")] // Примерный диапазон стоимости
        [Display(Name = "Стоимость Заказа")]
        public decimal Cost { get; set; }

        // Новое поле для комментариев менеджера
        [StringLength(500, ErrorMessage = "Комментарии менеджера не могут превышать 500 символов.")]
        [Display(Name = "Комментарии Менеджера")]
        public string? ManagerComments { get; set; } // Добавлен вопросительный знак для nullable строки

        // Списки для выпадающих меню
        public List<SelectListItem> Orders { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Masters { get; set; } = new List<SelectListItem>();
    }
}