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
            // Используем 'using' для области видимости, чтобы обеспечить правильное освобождение ресурсов
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServiceProvider = scope.ServiceProvider;
                var context = scopedServiceProvider.GetRequiredService<AccountDbContext>();
                var userManager = scopedServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scopedServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Убедитесь, что база данных создана или обновлена на основе миграций
                context.Database.Migrate();

                // --- Создание ролей для Калинковичского ремонтно-механического завода ---

                // Администратор
                if (!roleManager.RoleExistsAsync("Администратор").Result)
                    roleManager.CreateAsync(new IdentityRole("Администратор")).Wait();

                // МенеджерЗаказов (Order Manager)
                if (!roleManager.RoleExistsAsync("МенеджерЗаказов").Result)
                    roleManager.CreateAsync(new IdentityRole("МенеджерЗаказов")).Wait();

                // Мастер (Master)
                if (!roleManager.RoleExistsAsync("Мастер").Result)
                    roleManager.CreateAsync(new IdentityRole("Мастер")).Wait();

                // Клиент (Client) - НОВАЯ РОЛЬ ДОБАВЛЕНА ЗДЕСЬ
                if (!roleManager.RoleExistsAsync("Клиент").Result)
                    roleManager.CreateAsync(new IdentityRole("Клиент")).Wait();

                // --- Создание тестовых пользователей и назначение ролей ---

                // Администратор
                var adminEmail = "admin@krmz.by";
                if (userManager.FindByNameAsync(adminEmail).Result == null)
                {
                    var adminUser = new ApplicationUser { UserName = "Administrator", Email = adminEmail, EmailConfirmed = true };
                    var result = userManager.CreateAsync(adminUser, "Admin123").Result;
                    if (result.Succeeded)
                        userManager.AddToRoleAsync(adminUser, "Администратор").Wait();
                }

                // Менеджер Заказов
                var orderManagerEmail = "ordermanager@krmz.by";
                if (userManager.FindByNameAsync(orderManagerEmail).Result == null)
                {
                    var orderManagerUser = new ApplicationUser { UserName = "OrderManager", Email = orderManagerEmail, EmailConfirmed = true };
                    var result = userManager.CreateAsync(orderManagerUser, "Manager123").Result;
                    if (result.Succeeded)
                        userManager.AddToRoleAsync(orderManagerUser, "МенеджерЗаказов").Wait();
                }

                // Мастер
                var masterEmail = "master@krmz.by";
                if (userManager.FindByNameAsync(masterEmail).Result == null)
                {
                    var masterUser = new ApplicationUser { UserName = "Master", Email = masterEmail, EmailConfirmed = true };
                    var result = userManager.CreateAsync(masterUser, "Master123").Result;
                    if (result.Succeeded)
                        userManager.AddToRoleAsync(masterUser, "Мастер").Wait();
                }

                // Тестовый Клиент (Опционально: можно добавить тестового клиента)
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