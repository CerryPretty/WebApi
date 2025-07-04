using Microsoft.AspNetCore.Identity;

namespace WebUI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Adress { get; set; }
        public string Lastname { get; set; }

        // Конструктор по умолчанию
        public ApplicationUser()
        {
            Lastname = string.Empty;
            Adress = string.Empty;
        }

        // Если необходимо, можно добавить конструктор с параметрами
        public ApplicationUser(string lastname, string address)
        {
            Lastname = lastname;
            Adress = address;
        }
    }
}
