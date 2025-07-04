using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApi.Intarface; 
using WebApi.Models;
using WebUI.Models; 
using System.Globalization;

namespace WebUI.Controllers
{
    [Authorize(Roles = "МенеджерЗаказов")]
    public class OrderManagerController : Controller
    {
        private readonly ILogger<OrderManagerController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IOrderService _orderService; 
        private readonly UserManager<ApplicationUser> _userManager;
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

        public async Task<IActionResult> ViewAllOrders()
        {
            ViewData["Title"] = _localizer["Просмотр Всех Заказов"];
            try
            {
                var allWebApiOrders = _orderService.GetAllOrders();

                var allOrdersViewModels = new List<OrderViewModel>();
                foreach (var o in allWebApiOrders)
                {
                    string managerDisplayName = "N/A";
                    if (!string.IsNullOrEmpty(o.ManagerId))
                    {
                        var manager = await _userManager.FindByIdAsync(o.ManagerId); 
                        managerDisplayName = manager?.UserName ?? "N/A";
                    }

                    string masterDisplayName = "N/A";
                    if (!string.IsNullOrEmpty(o.MasterId))
                    {
                        var master = await _userManager.FindByIdAsync(o.MasterId);
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

        [HttpGet]
        public async Task<IActionResult> AssignMaster(int? orderId)
        {
            ViewData["Title"] = _localizer["Назначить Мастера"];

            try
            {
                var newOrders = _orderService.GetOrdersByStatusId(1);
                var masters = await _userManager.GetUsersInRoleAsync("Мастер"); 

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMaster(AssignMasterViewModel model)
        {
            ViewData["Title"] = _localizer["Назначить Мастера"];

            var newOrders = _orderService.GetOrdersByStatusId(1); 
            var masters = await _userManager.GetUsersInRoleAsync("Мастер"); 

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
                var orderToUpdate = _orderService.GetOrderById(model.SelectedOrderId); 

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

                var masterUser = await _userManager.FindByIdAsync(model.SelectedMasterId);
                if (masterUser == null || !(await _userManager.IsInRoleAsync(masterUser, "Мастер"))) 
                {
                    ModelState.AddModelError(string.Empty, _localizer["MasterNotFoundOrInvalid"].Value);
                    _logger.LogWarning($"Attempt to assign a non-existent master or a user without 'Мастер' role with ID: {model.SelectedMasterId}");
                    return View(model);
                }

                orderToUpdate.MasterId = model.SelectedMasterId;
                orderToUpdate.StatusId = 2;

                orderToUpdate.Cost = model.Cost;
                orderToUpdate.ManagerComments = model.ManagerComments;

                _orderService.UpdateOrder(orderToUpdate); 

                _logger.LogInformation($"Master {masterUser.UserName} assigned to order №{orderToUpdate.OrderNumber} (ID: {orderToUpdate.Id}). Status changed to 2. Cost updated to {orderToUpdate.Cost}. Manager comments: '{model.ManagerComments}'.");
                TempData["SuccessMessage"] = _localizer["MasterAssignedAndOrderUpdatedSuccessfully"].Value;
                return RedirectToAction("AssignMaster");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while assigning master and updating order ID: {model.SelectedOrderId}.");
                TempData["ErrorMessage"] = _localizer["ErrorAssigningMasterAndUpdatingOrder"].Value;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult GetOrderDetails(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId); 
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

        [HttpGet]
        public IActionResult ServiceCatalogManagement()
        {
            ViewData["Title"] = _localizer["Управление Каталогом Услуг"];
            try
            {
                var services = _serviceCatalogService.GetAll().ToList(); 

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
                };

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
                var serviceToDelete = _serviceCatalogService.Get(id);
                if (serviceToDelete == null)
                {
                    TempData["ErrorMessage"] = _localizer["ServiceNotFound"].Value;
                    return RedirectToAction(nameof(ServiceCatalogManagement));
                }

                _serviceCatalogService.Delete(id);
                TempData["SuccessMessage"] = _localizer["ServiceDeletedSuccessfully"].Value;
                _logger.LogInformation($"Service with ID: {id} deleted.");
                return RedirectToAction(nameof(ServiceCatalogManagement));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting service with ID: {id}.");
                TempData["ErrorMessage"] = _localizer["ErrorDeletingService"].Value;
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
                return RedirectToAction("Error"); 
            }
        }
    }
}