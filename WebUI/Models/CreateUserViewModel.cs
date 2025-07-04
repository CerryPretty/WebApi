using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
// Удаляем Microsoft.AspNetCore.Mvc.Rendering, так как AvailableRoles больше нет в этой ViewModel

namespace WebUI.Models
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно.")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Введите действительный адрес электронной почты.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        [StringLength(100, ErrorMessage = "{0} должен быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароль и пароль подтверждения не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Роли")]
        public List<string> SelectedRoles { get; set; } = new List<string>();

      
    }
}