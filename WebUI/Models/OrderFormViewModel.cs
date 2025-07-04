using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class OrderFormViewModel
    {
        
        public int ServiceId { get; set; }

        [Display(Name = "Название услуги")] 
        public string ServiceName { get; set; }

        [Display(Name = "Цена за единицу")] 
        [DataType(DataType.Currency)] 
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите количество.")] 
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1.")] 
        [Display(Name = "Количество")] 
        public int Quantity { get; set; } = 1; 

        [Required(ErrorMessage = "Обязательно опишите проблему.")] 
        [StringLength(1000, MinimumLength = 10,
                      ErrorMessage = "Описание проблемы должно быть от {2} до {1} символов.")] 
        [Display(Name = "Описание проблемы")] 
        [DataType(DataType.MultilineText)] 
        public string ProblemDescription { get; set; }
    }
}