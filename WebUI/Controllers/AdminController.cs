using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace WebUI.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            ViewData["Message"] = "Добро пожаловать в панель администратора!";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                userRoles[user.Id] = await _userManager.GetRolesAsync(user);
            }

            ViewData["UserRoles"] = userRoles;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Пользователь не найден.";
                return RedirectToAction(nameof(ManageUsers));
            }
            TempData["InfoMessage"] = $"Функция редактирования пользователя {user.UserName} пока не реализована.";
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Пользователь не найден.";
                return RedirectToAction(nameof(ManageUsers));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Пользователь '{user.UserName}' успешно удален.";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка при удалении пользователя.";
                foreach (var error in result.Errors)
                {
                    TempData["ErrorMessage"] += $" {error.Description}";
                }
            }
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            ViewData["AvailableRoles"] = (await _roleManager.Roles.ToListAsync())
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name });

            var model = new CreateUserViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            ViewData["AvailableRoles"] = (await _roleManager.Roles.ToListAsync())
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name });

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        var roleResult = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                        if (!roleResult.Succeeded)
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            await _userManager.DeleteAsync(user); 
                            TempData["ErrorMessage"] = "Ошибка при назначении ролей. Пользователь не был создан.";
                            return View(model); 
                        }
                    }

                    TempData["SuccessMessage"] = $"Пользователь '{user.UserName}' успешно создан.";
                    return RedirectToAction(nameof(ManageUsers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError(string.Empty, "Имя роли не может быть пустым.");
                return View();
            }

            if (await _roleManager.RoleExistsAsync(roleName.Trim()))
            {
                ModelState.AddModelError(string.Empty, $"Роль '{roleName.Trim()}' уже существует.");
                return View();
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Роль '{roleName.Trim()}' успешно создана.";
                return RedirectToAction(nameof(ManageRoles));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Роль не найдена.";
                return RedirectToAction(nameof(ManageRoles));
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                TempData["ErrorMessage"] = $"Невозможно удалить роль '{role.Name}', так как к ней привязаны пользователи ({usersInRole.Count()}). Сначала удалите пользователей из этой роли.";
                return RedirectToAction(nameof(ManageRoles));
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Роль '{role.Name}' успешно удалена.";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка при удалении роли.";
                foreach (var error in result.Errors)
                {
                    TempData["ErrorMessage"] += $" {error.Description}";
                }
            }
            return RedirectToAction(nameof(ManageRoles));
        }

        public IActionResult SystemSettings()
        {
            ViewData["Message"] = "Здесь будут системные настройки.";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}