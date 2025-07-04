using Microsoft.AspNetCore.Identity;

namespace WebUI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Adress { get; set; }
        public string Lastname { get; set; }

        public ApplicationUser()
        {
            Lastname = string.Empty;
            Adress = string.Empty;
        }

        public ApplicationUser(string lastname, string address)
        {
            Lastname = lastname;
            Adress = address;
        }
    }
}
