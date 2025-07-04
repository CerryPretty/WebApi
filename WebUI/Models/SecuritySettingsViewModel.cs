using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity; // Добавьте это, если ApplicationUser находится здесь

namespace WebUI.Models
{
    // Модель для объединения всех настроек безопасности на одной странице
    public class SecuritySettingsViewModel
    {
        public string CurrentUserName { get; set; }

        [Display(Name = "Изменить логин")]
        public ChangeUsernameViewModel ChangeUsername { get; set; } = new ChangeUsernameViewModel();

        [Display(Name = "Изменить пароль")]
        public ChangePasswordViewModel ChangePassword { get; set; } = new ChangePasswordViewModel();
    }

    // Модель для изменения логина
    public class ChangeUsernameViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите новый логин.")]
        [StringLength(256, ErrorMessage = "Логин должен быть не менее {2} и не более {1} символов.", MinimumLength = 3)]
        [Display(Name = "Новый логин")]
        // Можно добавить RegularExpression для более строгих правил именования логина:
        // [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Логин может содержать только буквы, цифры, символы '.', '-' и '_'")]
        public string NewUserName { get; set; }
    }

    // Модель для изменения пароля
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите текущий пароль.")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите новый пароль.")]
        [StringLength(100, ErrorMessage = "Пароль должен быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        // Важно: Эти атрибуты валидации на стороне клиента/модели.
        // Реальные требования к сложности пароля задаются в IdentityOptions в Startup.cs.
        // Ошибки от Identity будут добавлены в ModelState.
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmNewPassword { get; set; }
    }

    
   
   
}