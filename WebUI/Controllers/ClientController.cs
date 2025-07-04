using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Intarface;
using WebUI.Models;
using WebUI; // Убедитесь, что это содержит SharedResource и ApplicationUser
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace WebUI.Controllers
{
    [Authorize(Roles = "Клиент")]
    public class ClientController : Controller
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IServiceCatalogService _serviceCatalogService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager; // Добавлен SignInManager

        public ClientController(
            ILogger<ClientController> logger,
            IStringLocalizer<SharedResource> localizer,
            IServiceCatalogService serviceCatalogService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) // Инжектируем SignInManager
        {
            _logger = logger;
            _localizer = localizer;
            _serviceCatalogService = serviceCatalogService;
            _orderService = orderService;
            _userManager = userManager;
            _signInManager = signInManager; // Присваиваем
        }

        public IActionResult Index()
        {
            ViewData["Message"] = "Добро пожаловать в Ваш личный кабинет!";
            ViewData["Title"] = _localizer["ЛичныйКабинет"];
            return View();
        }

        public IActionResult MyProfile()
        {
            ViewData["Title"] = _localizer["МойПрофиль"];
            return View();
        }

        public async Task<IActionResult> MyOrders()
        {
            ViewData["Title"] = _localizer["МоиЗаказы"];

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Попытка доступа к заказам неавторизованным пользователем или пользователем без ID.");
                    TempData["ErrorMessage"] = _localizer["AuthenticationRequired"].Value;
                    return RedirectToAction("Login", "Account"); // Убедитесь, что у вас есть AccountController
                }

                var clientOrders = await _orderService.GetOrdersByClientId(userId);

                if (clientOrders == null || !clientOrders.Any())
                {
                    ViewData["NoOrdersMessage"] = _localizer["NoOrdersYet"].Value;
                    return View(new List<WebApi.Models.Order>());
                }

                return View(clientOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при загрузке заказов для клиента {User.Identity.Name}.");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingOrders"].Value;
                return RedirectToAction("Error");
            }
        }

        // НОВЫЙ Action для отображения страницы оплаты заказа
        [HttpGet]
        public async Task<IActionResult> PayOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Ошибка: ID пользователя не найден.";
                return RedirectToAction("Index", "Home");
            }

            // You should use an async method here if your service layer provides one.
            // For example: var order = await _orderService.GetAsync(orderId);
            var order = _orderService.Get(orderId); // Получаем заказ с услугами для отображения

            if (order == null || order.ClientId != userId || order.StatusId != 3) // Проверяем статус: только "Выполнен" (3) может быть оплачен
            {
                TempData["ErrorMessage"] = "Заказ не найден, не принадлежит вам, или не находится в статусе 'Выполнен' для оплаты.";
                return RedirectToAction("MyOrders");
            }

            string managerDisplayName = "N/A";
            if (!string.IsNullOrEmpty(order.ManagerId))
            {
                var managerUser = await _userManager.FindByIdAsync(order.ManagerId);
                managerDisplayName = managerUser?.UserName ?? "N/A";
            }

            string masterDisplayName = "N/A";
            if (!string.IsNullOrEmpty(order.MasterId))
            {
                var masterUser = await _userManager.FindByIdAsync(order.MasterId);
                masterDisplayName = masterUser?.UserName ?? "N/A";
            }

            var viewModel = new OrderDetailViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CreatedDate = order.CreatedDate,
                ClientDisplayName = order.ClientDisplayName ?? "N/A",
                ManagerDisplayName = managerDisplayName,
                MasterDisplayName = masterDisplayName,
                ProblemDescription = order.ProblemDescription,
                Status = order.Status,
                CompletionDate = order.CompletionDate,
                Cost = order.Cost,
                MasterComments = order.MasterComments,
                OrderServices = order.OrderServices
            };

            return View(viewModel);
        }

        [HttpPost] // Важно: этот Action должен вызываться POST-запросом
        [ValidateAntiForgeryToken] // Защита от CSRF
        public IActionResult ProcessPayment(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Пользователь не авторизован.";
                return RedirectToAction("MyOrders");
            }

            var orderToPay = _orderService.GetOrderById(orderId);

            // Проверяем, что заказ существует, принадлежит текущему клиенту и находится в статусе "Выполнен" (StatusId = 3)
            if (orderToPay == null || orderToPay.ClientId != userId || orderToPay.StatusId != 3)
            {
                TempData["ErrorMessage"] = "Заказ не найден, не принадлежит вам, или не находится в статусе 'Выполнен/Ожидает оплаты'.";
                return RedirectToAction("MyOrders");
            }

            // Изменяем статус заказа на "Оплачен" (предполагаем StatusId = 4)
            orderToPay.StatusId = 4;
            orderToPay.CompletionDate = DateTime.Now; // Устанавливаем дату завершения/оплаты

            try
            {
                _orderService.UpdateOrder(orderToPay); // Обновляем заказ в базе данных
                TempData["SuccessMessage"] = $"Заказ №{orderToPay.OrderNumber} успешно оплачен!";
            }
            catch (Exception ex)
            {
                // Логирование ошибки (для продакшна используйте настоящий логгер)
                Console.WriteLine($"Ошибка при оплате заказа {orderId}: {ex.Message}");
                TempData["ErrorMessage"] = $"Произошла ошибка при оплате заказа: {ex.Message}";
            }

            return RedirectToAction("MyOrders"); // Всегда перенаправляем на страницу "Мои заказы"
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder([FromBody] int orderId) // <--- Ключевое изменение здесь: [FromBody]
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Пользователь не авторизован." });
            }

            var orderToCancel = _orderService.GetOrderById(orderId);

            if (orderToCancel == null)
            {
                return Json(new { success = false, message = "Заказ не найден." });
            }

            // You should add a check here to ensure the order belongs to the current user
            // if you haven't already done so in GetOrderById.
            if (orderToCancel.ClientId != userId)
            {
                return Json(new { success = false, message = "У вас нет прав для отмены этого заказа." });
            }

            orderToCancel.StatusId = 5; // Assuming 5 is "Cancelled"
            orderToCancel.CompletionDate = DateTime.Now;

            try
            {
                _orderService.UpdateOrder(orderToCancel);
                return Json(new { success = true, message = $"Заказ №{orderToCancel.OrderNumber} успешно отменен." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка при отмене заказа: {ex.Message}" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> PlaceOrder(int serviceId)
        {
            ViewData["Title"] = _localizer["РазместитьЗаказ"];

            WebApi.Models.ServiceCatalog serviceToOrder = null;
            try
            {
                serviceToOrder = await _serviceCatalogService.GetServiceCatalogById(serviceId);
                if (serviceToOrder == null)
                {
                    _logger.LogWarning($"Service with ID: {serviceId} not found for order placement.");
                    TempData["ErrorMessage"] = _localizer["ServiceNotFoundForOrder"].Value;
                    return RedirectToAction("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching service {serviceId} for order placement.");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingServiceForOrder"].Value;
                return RedirectToAction("Error");
            }

            var orderFormViewModel = new OrderFormViewModel
            {
                ServiceId = serviceToOrder.Id,
                ServiceName = serviceToOrder.ServiceName,
                UnitPrice = serviceToOrder.Price,
                Quantity = 1,
                ProblemDescription = ""
            };

            return View(orderFormViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(OrderFormViewModel model)
        {
            ViewData["Title"] = _localizer["РазместитьЗаказ"];

            WebApi.Models.ServiceCatalog serviceToOrder = null;
            try
            {
                serviceToOrder = await _serviceCatalogService.GetServiceCatalogById(model.ServiceId);
                if (serviceToOrder == null)
                {
                    _logger.LogWarning($"Service with ID: {model.ServiceId} not found during order placement POST.");
                    TempData["ErrorMessage"] = _localizer["ServiceNotFoundForOrder"].Value;
                    return RedirectToAction("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error re-fetching service {model.ServiceId} for order placement POST.");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingServiceForOrder"].Value;
                return RedirectToAction("Error");
            }

            if (!ModelState.IsValid)
            {
                model.ServiceName = serviceToOrder.ServiceName;
                model.UnitPrice = serviceToOrder.Price;
                return View(model);
            }

            try
            {
                var managerUsers = await _userManager.GetUsersInRoleAsync("МенеджерЗаказов");
                string? managerId = managerUsers.FirstOrDefault()?.Id;

                if (string.IsNullOrEmpty(managerId))
                {
                    _logger.LogError("Не удалось определить ID менеджера по умолчанию для нового заказа.");
                    TempData["ErrorMessage"] = _localizer["ErrorNoManagerFound"].Value;
                    return RedirectToAction("Error");
                }

                // Calculate the total cost
                decimal totalCost = model.Quantity * model.UnitPrice;

                var newOrder = new WebApi.Models.Order
                {
                    OrderNumber = GenerateOrderNumber(),
                    CreatedDate = DateTime.Now,
                    ClientId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ClientDisplayName = User.Identity.Name,
                    StatusId = 1, // Предполагается, что 1 - это начальный статус, например "Новый"
                    ProblemDescription = model.ProblemDescription,
                    ManagerId = managerId,
                    Cost = totalCost // --- Assign the calculated total cost here ---
                };

                var orderServices = new List<WebApi.Models.OrderService>
                {
                    new WebApi.Models.OrderService
                    {
                        ServiceId = model.ServiceId,
                        Quantity = model.Quantity,
                        UnitPrice = model.UnitPrice
                    }
                };

                await _orderService.CreateOrderWithServices(newOrder, orderServices);

                _logger.LogInformation($"Order {newOrder.OrderNumber} for service ID {model.ServiceId} placed successfully by user {User.Identity.Name}. Total Cost: {totalCost:C}.");
                TempData["SuccessMessage"] = _localizer["OrderPlacedSuccessfully"].Value;
                return RedirectToAction("OrderConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error placing order for service ID: {model.ServiceId} by user {User.Identity.Name}.");
                TempData["ErrorMessage"] = _localizer["ErrorPlacingOrder"].Value;
                return RedirectToAction("Error");
            }
        }

        private string GenerateOrderNumber()
        {
            return "ORD-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }

        public IActionResult OrderConfirmation()
        {
            ViewData["Title"] = _localizer["ПодтверждениеЗаказа"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SecuritySettings()
        {
            ViewData["Title"] = _localizer["НастройкиБезопасности"];
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = _localizer["UserNotFound"].Value;
                return RedirectToAction("Index");
            }

            // Initialize both nested models for the GET request
            var model = new SecuritySettingsViewModel
            {
                CurrentUserName = user.UserName,
                ChangeUsername = new ChangeUsernameViewModel(),
                ChangePassword = new ChangePasswordViewModel()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ViewData["Title"] = _localizer["НастройкиБезопасности"];
            var userToUpdate = await _userManager.GetUserAsync(User);

            if (userToUpdate == null)
            {
                TempData["ErrorMessage"] = _localizer["UserNotFound"].Value;
                return RedirectToAction("Index");
            }

            // Re-fetch or create a complete SecuritySettingsViewModel for displaying errors
            var securitySettingsModel = new SecuritySettingsViewModel
            {
                CurrentUserName = userToUpdate.UserName,
                ChangePassword = model, // Assign the submitted model
                ChangeUsername = new ChangeUsernameViewModel() // Initialize the other form's model
            };

            if (!ModelState.IsValid)
            {
                // If the submitted password model is invalid, return the view with errors
                return View("SecuritySettings", securitySettingsModel);
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(userToUpdate, model.OldPassword);
            if (!isPasswordCorrect)
            {
                ModelState.AddModelError("ChangePassword.OldPassword", _localizer["IncorrectOldPassword"].Value);
                _logger.LogWarning($"Пользователь {userToUpdate.UserName} ввел неверный старый пароль.");
                return View("SecuritySettings", securitySettingsModel);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(userToUpdate, model.OldPassword, model.NewPassword);
            if (changePasswordResult.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(userToUpdate);
                await _signInManager.RefreshSignInAsync(userToUpdate);
                TempData["SuccessMessage"] = _localizer["PasswordChangedSuccessfully"].Value;
                _logger.LogInformation($"Пароль для пользователя {userToUpdate.UserName} успешно изменен.");
                return RedirectToAction("SecuritySettings");
            }

            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogWarning($"Ошибка смены пароля для {userToUpdate.UserName}: {error.Description}");
            }
            return View("SecuritySettings", securitySettingsModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUsername(ChangeUsernameViewModel model)
        {
            ViewData["Title"] = _localizer["НастройкиБезопасности"];
            var userToUpdate = await _userManager.GetUserAsync(User);

            if (userToUpdate == null)
            {
                TempData["ErrorMessage"] = _localizer["UserNotFound"].Value;
                return RedirectToAction("Index");
            }

            // Re-fetch or create a complete SecuritySettingsViewModel for displaying errors
            var securitySettingsModel = new SecuritySettingsViewModel
            {
                CurrentUserName = userToUpdate.UserName,
                ChangeUsername = model, // Assign the submitted model
                ChangePassword = new ChangePasswordViewModel() // Initialize the other form's model
            };

            if (!ModelState.IsValid)
            {
                // If the submitted username model is invalid, return the view with errors
                return View("SecuritySettings", securitySettingsModel);
            }

            if (userToUpdate.UserName == model.NewUserName)
            {
                ModelState.AddModelError("ChangeUsername.NewUserName", _localizer["NewUsernameSameAsCurrent"].Value);
                _logger.LogWarning($"Пользователь {userToUpdate.UserName} попытался сменить логин на текущий.");
                return View("SecuritySettings", securitySettingsModel);
            }

            var existingUserWithNewUsername = await _userManager.FindByNameAsync(model.NewUserName);
            if (existingUserWithNewUsername != null && existingUserWithNewUsername.Id != userToUpdate.Id)
            {
                ModelState.AddModelError("ChangeUsername.NewUserName", _localizer["UsernameAlreadyTaken"].Value);
                _logger.LogWarning($"Пользователь {userToUpdate.UserName} попытался сменить логин на уже занятый: {model.NewUserName}.");
                return View("SecuritySettings", securitySettingsModel);
            }

            var setUserNameResult = await _userManager.SetUserNameAsync(userToUpdate, model.NewUserName);
            if (setUserNameResult.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(userToUpdate);
                await _signInManager.RefreshSignInAsync(userToUpdate);
                TempData["SuccessMessage"] = _localizer["UsernameChangedSuccessfully"].Value;
                _logger.LogInformation($"Логин пользователя {userToUpdate.UserName} успешно изменен на {model.NewUserName}.");
                return RedirectToAction("SecuritySettings");
            }

            foreach (var error in setUserNameResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                if (error.Code == "DuplicateUserName")
                {
                    ModelState.AddModelError("ChangeUsername.NewUserName", error.Description);
                }
                _logger.LogWarning($"Ошибка смены логина для {userToUpdate.UserName}: {error.Description}");
            }
            return View("SecuritySettings", securitySettingsModel);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var errorViewModel = new ErrorViewModel { RequestId = requestId };

            if (TempData["ErrorMessage"] != null)
            {
                errorViewModel.Message = TempData["ErrorMessage"].ToString();
            }

            return View(errorViewModel);
        }
    }
}