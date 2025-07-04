using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using WebApi.Intarface;
using WebApi.Models;
using WebUI.Models;

namespace WebUI.Controllers
{
    [Authorize(Roles = "Мастер")]
    public class MasterController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public MasterController(IOrderService orderService, UserManager<ApplicationUser> userManager, IStringLocalizer<SharedResource> localizer)
        {
            _orderService = orderService;
            _userManager = userManager;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            ViewData["Message"] = "Добро пожаловать в панель Мастера!";
            return View();
        }

        public async Task<IActionResult> AssignedTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Ошибка: ID пользователя не найден.";
                return RedirectToAction("Index");
            }

            var orders = _orderService.GetOrdersByMasterId(userId)
                                      .Where(o => o.StatusId == 2)
                                      .ToList();

            var orderViewModels = await Task.WhenAll(orders.Select(async o =>
            {
                var managerUser = o.ManagerId != null ? await _userManager.FindByIdAsync(o.ManagerId) : null;
                var masterUser = o.MasterId != null ? await _userManager.FindByIdAsync(o.MasterId) : null;

                var serviceNames = o.OrderServices
                                        .Where(os => os.Service != null)
                                        .Select(os => os.Service.ServiceName)
                                        .ToList();
                var combinedServiceNames = serviceNames.Any() ? string.Join(", ", serviceNames) : _localizer["Не указана"].Value;


                return new OrderViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CreatedDate = o.CreatedDate,
                    ClientDisplayName = o.ClientDisplayName ?? "N/A",
                    ManagerDisplayName = managerUser?.UserName ?? "N/A",
                    MasterDisplayName = masterUser?.UserName ?? "N/A",
                    ServiceName = combinedServiceNames,
                    ProblemDescription = o.ProblemDescription,
                    Status = o.Status ?? new OrderStatus { Id = o.StatusId, StatusName = GetStatusNameById(o.StatusId) },
                    Cost = o.Cost,
                    ManagerComments = o.ManagerComments,
                    MasterComments = o.MasterComments
                };
            }));

            var currentUser = await _userManager.GetUserAsync(User);
            var masterDisplayName = currentUser?.UserName ?? "Неизвестный мастер";

            var viewModel = new MasterAssignedTasksViewModel
            {
                AssignedOrders = orderViewModels,
                MasterDisplayName = masterDisplayName
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartReportForOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orderToReport = _orderService.GetOrderById(orderId);

            if (orderToReport == null || orderToReport.MasterId != userId || orderToReport.StatusId != 2)
            {
                TempData["ErrorMessage"] = "Ошибка: Заказ не найден, не принадлежит вам или не находится в статусе 'В работе'.";
                return RedirectToAction("AssignedTasks");
            }

            return RedirectToAction("ReportProduction", new { orderId = orderId });
        }

        [HttpGet]
        public async Task<IActionResult> ReportProduction(int? orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Ошибка: ID пользователя не найден.";
                return RedirectToAction("Index");
            }

            var masterOrders = _orderService.GetOrdersByMasterId(userId)
                                             .Where(o => o.StatusId == 2)
                                             .ToList();

            var orderListItems = masterOrders.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = $"Заказ №{o.OrderNumber} (Клиент: {o.ClientDisplayName ?? "N/A"})"
            }).ToList();

            var viewModel = new ReportProductionViewModel
            {
                CompletedOrders = orderListItems,
                SelectedOrderId = orderId
            };

            if (orderId.HasValue)
            {
                var selectedOrder = masterOrders.FirstOrDefault(o => o.Id == orderId.Value);

                if (selectedOrder != null && selectedOrder.MasterId == userId)
                {
                    viewModel.MasterComments = selectedOrder.MasterComments;
                    viewModel.Cost = selectedOrder.Cost;
                    viewModel.ManagerComments = selectedOrder.ManagerComments;

                    var serviceNames = selectedOrder.OrderServices
                                                     .Where(os => os.Service != null)
                                                     .Select(os => os.Service.ServiceName)
                                                     .ToList();
                    viewModel.ServiceNamesForSelectedOrder = serviceNames.Any() ? string.Join(", ", serviceNames) : _localizer["Не указана"].Value;
                }
                else
                {
                    TempData["ErrorMessage"] = "Ошибка: Выбранный заказ не найден или не принадлежит вам.";
                    return RedirectToAction("AssignedTasks");
                }
            }
            else if (!orderListItems.Any())
            {
                TempData["InfoMessage"] = "У вас нет активных заказов, ожидающих отчета.";
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReportProduction(ReportProductionViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            model.CompletedOrders = _orderService.GetOrdersByMasterId(userId)
                                                     .Where(o => o.StatusId == 2)
                                                     .ToList()
                                                     .Select(o => new SelectListItem
                                                     {
                                                         Value = o.Id.ToString(),
                                                         Text = $"Заказ №{o.OrderNumber} (Клиент: {o.ClientDisplayName ?? "N/A"})"
                                                     }).ToList();

            if (!ModelState.IsValid)
            {
                if (model.SelectedOrderId.HasValue)
                {
                    var selectedOrder = _orderService.GetOrderById(model.SelectedOrderId.Value); 
                    if (selectedOrder != null && selectedOrder.MasterId == userId)
                    {
                        var serviceNames = selectedOrder.OrderServices
                                                         .Where(os => os.Service != null)
                                                         .Select(os => os.Service.ServiceName)
                                                         .ToList();
                        model.ServiceNamesForSelectedOrder = serviceNames.Any() ? string.Join(", ", serviceNames) : _localizer["Не указана"].Value;
                        model.ManagerComments = selectedOrder.ManagerComments; 
                        model.Cost = selectedOrder.Cost; 
                    }
                }
                TempData["ErrorMessage"] = "Пожалуйста, исправьте ошибки в форме.";
                return View(model);
            }

            var orderToUpdate = _orderService.GetOrderById(model.SelectedOrderId ?? 0);

            if (orderToUpdate == null || orderToUpdate.MasterId != userId || orderToUpdate.StatusId != 2)
            {
                TempData["ErrorMessage"] = "Ошибка: Выбранный заказ не найден, не принадлежит вам или не находится в статусе 'В работе'.";
                return RedirectToAction("AssignedTasks");
            }

            orderToUpdate.MasterComments = model.MasterComments;
            orderToUpdate.Cost = model.Cost;

            orderToUpdate.StatusId = 3;
            orderToUpdate.CompletionDate = DateTime.Now;

            try
            {
                _orderService.UpdateOrder(orderToUpdate);
                TempData["SuccessMessage"] = $"Отчет по заказу №{orderToUpdate.OrderNumber} успешно сохранен. Фактическая стоимость: {orderToUpdate.Cost:C}. Заказ теперь в статусе '{GetStatusNameById(orderToUpdate.StatusId)}'.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Не удалось сохранить отчет по заказу №{orderToUpdate.OrderNumber}: {ex.Message}";
            }

            return RedirectToAction("AssignedTasks");
        }


        private string GetStatusNameById(int statusId)
        {
            return statusId switch
            {
                1 => "Новый",
                2 => "В работе",
                3 => "Выполнен",
                _ => "Неизвестно"
            };
        }
    }
}