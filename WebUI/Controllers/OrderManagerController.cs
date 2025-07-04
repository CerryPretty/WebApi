using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks; // Still needed for UserManager and potentially IOrderService
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using WebApi.Intarface; // For IServiceCatalogService and IService
using WebApi.Models;
using WebUI.Models; // Assuming ServiceCatalogViewModel and ErrorViewModel are here
using System;
using System.Globalization;

namespace WebUI.Controllers
{
    [Authorize(Roles = "МенеджерЗаказов")]
    public class OrderManagerController : Controller
    {
        private readonly ILogger<OrderManagerController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IOrderService _orderService; // Assuming this service might have async methods
        private readonly UserManager<ApplicationUser> _userManager; // UserManager is inherently async
        private readonly IServiceCatalogService _serviceCatalogService;

        public OrderManagerController(
            ILogger<OrderManagerController> logger,
            IStringLocalizer<SharedResource> localizer,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager,
            IServiceCatalogService serviceCatalogService)
        {
            _logger = logger;
            _localizer = localizer;
            _orderService = orderService;
            _userManager = userManager;
            _serviceCatalogService = serviceCatalogService;
        }

        public IActionResult Index()
        {
            ViewData["Message"] = _localizer["Добро пожаловать в раздел управления заказами!"];
            ViewData["Title"] = _localizer["Управление Заказами"];
            return View();
        }

        // This action *must* remain async because of UserManager.FindByIdAsync
        public async Task<IActionResult> ViewAllOrders()
        {
            ViewData["Title"] = _localizer["Просмотр Всех Заказов"];
            try
            {
                // Assuming _orderService.GetAllOrders() is synchronous or needs no await
                var allWebApiOrders = _orderService.GetAllOrders();

                var allOrdersViewModels = new List<OrderViewModel>();
                foreach (var o in allWebApiOrders)
                {
                    string managerDisplayName = "N/A";
                    if (!string.IsNullOrEmpty(o.ManagerId))
                    {
                        var manager = await _userManager.FindByIdAsync(o.ManagerId); // AWAIT still needed here
                        managerDisplayName = manager?.UserName ?? "N/A";
                    }

                    string masterDisplayName = "N/A";
                    if (!string.IsNullOrEmpty(o.MasterId))
                    {
                        var master = await _userManager.FindByIdAsync(o.MasterId); // AWAIT still needed here
                        masterDisplayName = master?.UserName ?? "N/A";
                    }

                    allOrdersViewModels.Add(new OrderViewModel
                    {
                        Id = o.Id,
                        OrderNumber = o.OrderNumber,
                        CreatedDate = o.CreatedDate,
                        ClientDisplayName = o.ClientDisplayName ?? "N/A",
                        ManagerDisplayName = managerDisplayName,
                        MasterDisplayName = masterDisplayName,
                        ProblemDescription = o.ProblemDescription,
                        Status = o.Status,
                        Cost = o.Cost,
                        ManagerComments = o.ManagerComments,
                        MasterComments = o.MasterComments
                    });
                }

                var ordersByStatus = allWebApiOrders
                    .GroupBy(o => o.Status?.StatusName ?? _localizer["Неизвестный статус"].Value)
                    .ToDictionary(g => g.Key, g => g.Count());

                var ordersCreatedMonthly = allWebApiOrders
                    .GroupBy(o => o.CreatedDate.ToString("yyyy-MM", CultureInfo.InvariantCulture))
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Count());

                var viewModel = new OrderManagerOrdersViewModel
                {
                    AllOrders = allOrdersViewModels.OrderByDescending(o => o.CreatedDate).ToList(),
                    OrdersByStatus = ordersByStatus,
                    OrdersCreatedMonthly = ordersCreatedMonthly
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading all orders and preparing report data.");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingOrders"].Value;
                return RedirectToAction("Error");
            }
        }

        // This action *must* remain async because of UserManager.GetUsersInRoleAsync
        [HttpGet]
        public async Task<IActionResult> AssignMaster(int? orderId)
        {
            ViewData["Title"] = _localizer["Назначить Мастера"];

            try
            {
                // Assuming _orderService.GetOrdersByStatusId(1) is synchronous
                var newOrders = _orderService.GetOrdersByStatusId(1);
                var masters = await _userManager.GetUsersInRoleAsync("Мастер"); // AWAIT still needed here

                if (!newOrders.Any())
                {
                    TempData["WarningMessage"] = _localizer["NoNewOrdersToAssign"].Value;
                }
                if (!masters.Any())
                {
                    TempData["ErrorMessage"] = _localizer["NoMastersFound"].Value;
                }

                var orderListItems = newOrders.Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = $"{_localizer["Заказ"]} №{o.OrderNumber} ({_localizer["Клиент"]}: {o.ClientDisplayName ?? "N/A"})"
                }).ToList();

                var masterListItems = masters.Select(m => new SelectListItem
                {
                    Value = m.Id,
                    Text = m.UserName
                }).ToList();

                var model = new AssignMasterViewModel
                {
                    Orders = orderListItems,
                    Masters = masterListItems
                };

                if (orderId.HasValue && orderListItems.Any(o => o.Value == orderId.Value.ToString()))
                {
                    model.SelectedOrderId = orderId.Value;
                    // Assuming _orderService.GetOrderById(orderId.Value) is synchronous
                    var preselectedOrder = _orderService.GetOrderById(orderId.Value);
                    if (preselectedOrder != null)
                    {
                        model.Cost = preselectedOrder.Cost ?? 0m;
                        model.ManagerComments = preselectedOrder.ManagerComments;
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading data for the assign master page.");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingData"].Value;
                return RedirectToAction("Error");
            }
        }

        // This action *must* remain async because of UserManager.FindByIdAsync and GetUsersInRoleAsync
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMaster(AssignMasterViewModel model)
        {
            ViewData["Title"] = _localizer["Назначить Мастера"];

            // Re-populate dropdowns in case of ModelState errors
            var newOrders = _orderService.GetOrdersByStatusId(1); // Assuming synchronous
            var masters = await _userManager.GetUsersInRoleAsync("Мастер"); // AWAIT still needed here

            model.Orders = newOrders.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = $"{_localizer["Заказ"]} №{o.OrderNumber} ({_localizer["Клиент"]}: {o.ClientDisplayName ?? "N/A"})"
            }).ToList();
            model.Masters = masters.Select(m => new SelectListItem
            {
                Value = m.Id,
                Text = m.UserName
            }).ToList();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = _localizer["PleaseCorrectErrors"].Value;
                return View(model);
            }

            try
            {
                var orderToUpdate = _orderService.GetOrderById(model.SelectedOrderId); // Assuming synchronous

                if (orderToUpdate == null)
                {
                    ModelState.AddModelError(string.Empty, _localizer["OrderNotFound"].Value);
                    _logger.LogWarning($"Attempt to assign master to a non-existent order with ID: {model.SelectedOrderId}");
                    return View(model);
                }

                if (orderToUpdate.StatusId != 1)
                {
                    ModelState.AddModelError(string.Empty, _localizer["OrderIsNotNew"].Value);
                    _logger.LogWarning($"Attempt to assign master to order ID: {model.SelectedOrderId}, which is not new (current status: {orderToUpdate.StatusId}).");
                    return View(model);
                }

                var masterUser = await _userManager.FindByIdAsync(model.SelectedMasterId); // AWAIT still needed here
                if (masterUser == null || !(await _userManager.IsInRoleAsync(masterUser, "Мастер"))) // AWAIT still needed here
                {
                    ModelState.AddModelError(string.Empty, _localizer["MasterNotFoundOrInvalid"].Value);
                    _logger.LogWarning($"Attempt to assign a non-existent master or a user without 'Мастер' role with ID: {model.SelectedMasterId}");
                    return View(model);
                }

                orderToUpdate.MasterId = model.SelectedMasterId;
                orderToUpdate.StatusId = 2;

                orderToUpdate.Cost = model.Cost;
                orderToUpdate.ManagerComments = model.ManagerComments;

                _orderService.UpdateOrder(orderToUpdate); // Assuming synchronous

                _logger.LogInformation($"Master {masterUser.UserName} assigned to order №{orderToUpdate.OrderNumber} (ID: {orderToUpdate.Id}). Status changed to 2. Cost updated to {orderToUpdate.Cost}. Manager comments: '{model.ManagerComments}'.");
                TempData["SuccessMessage"] = _localizer["MasterAssignedAndOrderUpdatedSuccessfully"].Value;
                return RedirectToAction("AssignMaster");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while assigning master and updating order ID: {model.SelectedOrderId}.");
                // Corrected line: Directly use .Value property to get the string from LocalizedString
                TempData["ErrorMessage"] = _localizer["ErrorAssigningMasterAndUpdatingOrder"].Value;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult GetOrderDetails(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId); // Assuming synchronous
                if (order != null)
                {
                    return Json(new { cost = order.Cost ?? 0m, managerComments = order.ManagerComments });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching details for order ID: {orderId}");
                return StatusCode(500, "Internal server error.");
            }
        }

        public IActionResult ProcessOrder(int id)
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            var requestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var errorViewModel = new ErrorViewModel { RequestId = requestId };

            if (TempData["ErrorMessage"] != null)
            {
                errorViewModel.Message = TempData["ErrorMessage"].ToString();
            }

            return View(errorViewModel);
        }

        // --- Service Catalog Actions (All Synchronous) ---

        [HttpGet]
        public IActionResult ServiceCatalogManagement()
        {
            ViewData["Title"] = _localizer["Управление Каталогом Услуг"];
            try
            {
                // Calling the synchronous GetAll() from the base interface
                var services = _serviceCatalogService.GetAll().ToList(); // Convert IQueryable to List for view model

                var serviceViewModels = services.Select(s => new ServiceCatalogViewModel
                {
                    Id = s.Id,
                    ServiceName = s.ServiceName,
                    Price = s.Price,
                    Category = s.Category,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl
                }).ToList();
                return View(serviceViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading service catalog data.");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingServiceCatalog"].Value;
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public IActionResult CreateService()
        {
            ViewData["Title"] = _localizer["Добавить Новую Услугу"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateService(ServiceCatalogViewModel model)
        {
            ViewData["Title"] = _localizer["Добавить Новую Услугу"];
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = _localizer["PleaseCorrectErrors"].Value;
                return View(model);
            }

            try
            {
                var service = new ServiceCatalog
                {
                    ServiceName = model.ServiceName,
                    Price = model.Price,
                    Category = model.Category,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl
                    // CreatedAt will be set in the service
                };

                // Calling the synchronous Create() from the base interface
                _serviceCatalogService.Create(service);
                TempData["SuccessMessage"] = _localizer["ServiceAddedSuccessfully"].Value;
                _logger.LogInformation($"New service '{service.ServiceName}' added to catalog.");
                return RedirectToAction(nameof(ServiceCatalogManagement));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating new service '{model.ServiceName}'.");
                TempData["ErrorMessage"] = _localizer["ErrorAddingService"].Value;
                return View(model);
            }
        }
        
        [HttpGet]
        public IActionResult EditService(int id)
        {
            ViewData["Title"] = _localizer["Редактировать Услугу"];
            try
            {
                // Calling the synchronous Get(int id) from the base interface
                var service = _serviceCatalogService.Get(id);

                if (service == null)
                {
                    TempData["ErrorMessage"] = _localizer["ServiceNotFound"].Value;
                    return RedirectToAction(nameof(ServiceCatalogManagement));
                }

                var model = new ServiceCatalogViewModel
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    Price = service.Price,
                    Category = service.Category,
                    Description = service.Description,
                    ImageUrl = service.ImageUrl
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading service for edit, ID: {id}.");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingService"].Value;
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditService(ServiceCatalogViewModel model)
        {
            ViewData["Title"] = _localizer["Редактировать Услугу"];
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = _localizer["PleaseCorrectErrors"].Value;
                return View(model);
            }

            try
            {
                // Calling the synchronous Get(int id) from the base interface
                var serviceToUpdate = _serviceCatalogService.Get(model.Id);
                if (serviceToUpdate == null)
                {
                    TempData["ErrorMessage"] = _localizer["ServiceNotFound"].Value;
                    return RedirectToAction(nameof(ServiceCatalogManagement));
                }

                serviceToUpdate.ServiceName = model.ServiceName;
                serviceToUpdate.Price = model.Price;
                serviceToUpdate.Category = model.Category;
                serviceToUpdate.Description = model.Description;
                serviceToUpdate.ImageUrl = model.ImageUrl;
                // UpdatedAt will be set in the service

                // Calling the synchronous Update() from the base interface
                _serviceCatalogService.Update(serviceToUpdate);
                TempData["SuccessMessage"] = _localizer["ServiceUpdatedSuccessfully"].Value;
                _logger.LogInformation($"Service '{serviceToUpdate.ServiceName}' (ID: {serviceToUpdate.Id}) updated.");
                return RedirectToAction(nameof(ServiceCatalogManagement));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating service '{model.ServiceName}' (ID: {model.Id}).");
                TempData["ErrorMessage"] = _localizer["ErrorUpdatingService"].Value;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult DeleteService(int id)
        {
            ViewData["Title"] = _localizer["Удалить Услугу"];
            try
            {
                // Calling the synchronous Get(int id) from the base interface
                var service = _serviceCatalogService.Get(id);

                if (service == null)
                {
                    TempData["ErrorMessage"] = _localizer["ServiceNotFound"].Value;
                    return RedirectToAction(nameof(ServiceCatalogManagement));
                }

                var model = new ServiceCatalogViewModel
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    Price = service.Price,
                    Category = service.Category,
                    Description = service.Description,
                    ImageUrl = service.ImageUrl
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading service for deletion, ID: {id}.");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingService"].Value;
                return RedirectToAction("Error");
            }
        }

        [HttpPost, ActionName("DeleteService")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteServiceConfirmed(int id)
        {
            ViewData["Title"] = _localizer["Удалить Услугу"];
            try
            {
                // First, try to get the service to ensure it exists before deleting
                var serviceToDelete = _serviceCatalogService.Get(id);
                if (serviceToDelete == null)
                {
                    TempData["ErrorMessage"] = _localizer["ServiceNotFound"].Value;
                    return RedirectToAction(nameof(ServiceCatalogManagement));
                }

                // Calling the synchronous Delete() from the base interface
                _serviceCatalogService.Delete(id);
                TempData["SuccessMessage"] = _localizer["ServiceDeletedSuccessfully"].Value;
                _logger.LogInformation($"Service with ID: {id} deleted.");
                return RedirectToAction(nameof(ServiceCatalogManagement));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting service with ID: {id}.");
                TempData["ErrorMessage"] = _localizer["ErrorDeletingService"].Value;
                // If deletion fails, fetch the service again to display on the view
                var serviceToDisplay = _serviceCatalogService.Get(id);
                if (serviceToDisplay != null)
                {
                    var model = new ServiceCatalogViewModel
                    {
                        Id = serviceToDisplay.Id,
                        ServiceName = serviceToDisplay.ServiceName,
                        Price = serviceToDisplay.Price,
                        Category = serviceToDisplay.Category,
                        Description = serviceToDisplay.Description,
                        ImageUrl = serviceToDisplay.ImageUrl
                    };
                    return View(model);
                }
                return RedirectToAction("Error"); // Fallback if service can't be found even for display
            }
        }
    }
}