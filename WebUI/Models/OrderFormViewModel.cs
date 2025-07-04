using System.ComponentModel.DataAnnotations;
using System.Security.Claims; // Это using не влияет на саму ViewModel, но часто используется с моделями представлений
// using WebUI; // Удалено, так как SharedResource больше не используется

namespace WebUI.Models
{
    public class OrderFormViewModel
    {
        // ServiceId и ServiceName передаются из представления и не требуют валидации
        // так как они readonly и служат для отображения информации о выбранной услуге.
        public int ServiceId { get; set; }

        [Display(Name = "Название услуги")] // Отображаемое имя для UI
        public string ServiceName { get; set; }

        [Display(Name = "Цена за единицу")] // Отображаемое имя для UI
        [DataType(DataType.Currency)] // Указывает, что это валюта, для форматирования в UI
        public decimal UnitPrice { get; set; }

        // --- Важные изменения для валидации ---
        [Required(ErrorMessage = "Пожалуйста, укажите количество.")] // Делает поле обязательным
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1.")] // Устанавливает диапазон значений
        [Display(Name = "Количество")] // Отображаемое имя для UI
        public int Quantity { get; set; } = 1; // Установлено значение по умолчанию для удобства

        [Required(ErrorMessage = "Обязательно опишите проблему.")] // Делает поле обязательным
        [StringLength(1000, MinimumLength = 10,
                      ErrorMessage = "Описание проблемы должно быть от {2} до {1} символов.")] // Ограничивает длину строки и обеспечивает минимальную длину
        [Display(Name = "Описание проблемы")] // Отображаемое имя для UI
        [DataType(DataType.MultilineText)] // Указывает, что это многострочный текст (для textarea в UI)
        public string ProblemDescription { get; set; }

       
    }
}