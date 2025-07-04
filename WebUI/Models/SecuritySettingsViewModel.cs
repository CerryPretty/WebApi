using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class SecuritySettingsViewModel
    {
        public string CurrentUserName { get; set; }

        [Display(Name = "Изменить логин")]
        public ChangeUsernameViewModel ChangeUsername { get; set; } = new ChangeUsernameViewModel();

        [Display(Name = "Изменить пароль")]
        public ChangePasswordViewModel ChangePassword { get; set; } = new ChangePasswordViewModel();
    }

    public class ChangeUsernameViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите новый логин.")]
        [StringLength(256, ErrorMessage = "Логин должен быть не менее {2} и не более {1} символов.", MinimumLength = 3)]
        [Display(Name = "Новый логин")]
        public string NewUserName { get; set; }
    }

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
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmNewPassword { get; set; }
    }

    
   
   
}