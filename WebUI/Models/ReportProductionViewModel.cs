using Microsoft.AspNetCore.Mvc.Rendering; 
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class ReportProductionViewModel
    {
        [Display(Name = "Выберите Заказ")]
        [Required(ErrorMessage = "Необходимо выбрать заказ.")]
        public int? SelectedOrderId { get; set; }
        public string ManagerComments { get; set; }
       
        [Display(Name = "Названия Услуг")]
        public string? ServiceNamesForSelectedOrder { get; set; }

        public List<SelectListItem> CompletedOrders { get; set; } = new List<SelectListItem>();

        [Display(Name = "Комментарии Мастера")]
        [StringLength(1000, ErrorMessage = "Длина комментария не должна превышать 1000 символов.")]
        public string? MasterComments { get; set; }

        [Display(Name = "Фактическая Стоимость")] 
        [DataType(DataType.Currency)]
        [Range(0.00, 1000000.00, ErrorMessage = "Стоимость должна быть положительным числом или нулем.")]
        public decimal? Cost { get; set; }
    }
}
