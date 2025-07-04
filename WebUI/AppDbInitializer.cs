using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebUI.DbContext;
using WebUI.Models;

namespace WebUI
{
    public class AppDbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServiceProvider = scope.ServiceProvider;
                var context = scopedServiceProvider.GetRequiredService<AccountDbContext>();
                var userManager = scopedServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scopedServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                context.Database.Migrate();

                if (!roleManager.RoleExistsAsync("Администратор").Result)
                    roleManager.CreateAsync(new IdentityRole("Администратор")).Wait();

                if (!roleManager.RoleExistsAsync("МенеджерЗаказов").Result)
                    roleManager.CreateAsync(new IdentityRole("МенеджерЗаказов")).Wait();

                if (!roleManager.RoleExistsAsync("Мастер").Result)
                    roleManager.CreateAsync(new IdentityRole("Мастер")).Wait();

                if (!roleManager.RoleExistsAsync("Клиент").Result)
                    roleManager.CreateAsync(new IdentityRole("Клиент")).Wait();

                var adminEmail = "admin@krmz.by";
                if (userManager.FindByNameAsync(adminEmail).Result == null)
                {
                    var adminUser = new ApplicationUser { UserName = "Administrator", Email = adminEmail, EmailConfirmed = true };
                    var result = userManager.CreateAsync(adminUser, "Admin123").Result;
                    if (result.Succeeded)
                        userManager.AddToRoleAsync(adminUser, "Администратор").Wait();
                }

                var orderManagerEmail = "ordermanager@krmz.by";
                if (userManager.FindByNameAsync(orderManagerEmail).Result == null)
                {
                    var orderManagerUser = new ApplicationUser { UserName = "OrderManager", Email = orderManagerEmail, EmailConfirmed = true };
                    var result = userManager.CreateAsync(orderManagerUser, "Manager123").Result;
                    if (result.Succeeded)
                        userManager.AddToRoleAsync(orderManagerUser, "МенеджерЗаказов").Wait();
                }

                var masterEmail = "master@krmz.by";
                if (userManager.FindByNameAsync(masterEmail).Result == null)
                {
                    var masterUser = new ApplicationUser { UserName = "Master", Email = masterEmail, EmailConfirmed = true };
                    var result = userManager.CreateAsync(masterUser, "Master123").Result;
                    if (result.Succeeded)
                        userManager.AddToRoleAsync(masterUser, "Мастер").Wait();
                }

                var clientEmail = "client@krmz.by";
                if (userManager.FindByNameAsync(clientEmail).Result == null)
                {
                    var clientUser = new ApplicationUser { UserName = "ClientUser", Email = clientEmail, EmailConfirmed = true };
                    var result = userManager.CreateAsync(clientUser, "Client123").Result;
                    if (result.Succeeded)
                        userManager.AddToRoleAsync(clientUser, "Клиент").Wait();
                }
            }
        }
    }
}