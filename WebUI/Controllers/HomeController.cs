using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebUI.Models; 
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Text;
using WebApi.Models; 
using WebApi.Intarface; 
using System.Text.RegularExpressions; 

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceCatalogService _serviceCatalogService; 

        public HomeController(
            ILogger<HomeController> logger,
            IStringLocalizer<SharedResource> localizer,
            IHttpClientFactory httpClientFactory,
            IServiceCatalogService serviceCatalogService) 
        {
            _logger = logger;
            _localizer = localizer;
            _httpClientFactory = httpClientFactory;
            _serviceCatalogService = serviceCatalogService; 
        }

        public IActionResult Index()
        {
            ViewData["Title"] = _localizer["Welcome"];
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = _localizer["PrivacyPolicy"];
            return View();
        }

        public async Task<IActionResult> Catalog()
        {
            ViewData["Title"] = _localizer["Catalog"];
            List<ServiceCatalog> serviceCatalogs = new List<ServiceCatalog>();

            try
            {
                _logger.LogInformation($"Получение каталога услуг напрямую через сервис.");

                var result = await _serviceCatalogService.GetAllServiceCatalogs();
                serviceCatalogs = result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении каталога услуг через сервис.");
                ViewData["ErrorMessage"] = _localizer["ErrorLoadingCatalog"];
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = _localizer["ErrorLoadingCatalog"] 
                });
            }

            return View(serviceCatalogs);
        }

        public async Task<IActionResult> ServiceDetails(int id)
        {
            ViewData["Title"] = _localizer["ДеталиУслуги"];
            ServiceCatalog service = null;

            try
            {
                _logger.LogInformation($"Получение деталей услуги с ID: {id} напрямую через сервис.");
                service = await _serviceCatalogService.GetServiceCatalogById(id);

                if (service == null)
                {
                    _logger.LogWarning($"Услуга с ID: {id} не найдена.");
                    ViewData["ErrorMessage"] = _localizer["ServiceNotFound"];
                    return View("Error", new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                        Message = _localizer["ServiceNotFound"]
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении деталей услуги с ID: {id} через сервис.");
                ViewData["ErrorMessage"] = _localizer["ErrorLoadingServiceDetails"];
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = _localizer["ErrorLoadingServiceDetails"]
                });
            }
            return View(service);
        }

        public IActionResult History()
        {
            ViewData["Title"] = _localizer["History"];
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = _localizer["Contacts"];
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorMessage = ViewData["ErrorMessage"] as string;
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = errorMessage 
            });
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            _logger.LogInformation($"Attempting to set culture to: {culture} for return URL: {returnUrl}");

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true }
            );

            _logger.LogInformation($"Culture cookie set. Redirecting to: {returnUrl}");

            return LocalRedirect(returnUrl);
        }

        [HttpGet("api/krmz-news")]
        public async Task<IActionResult> GetKrmzNews()
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync("https://www.krmz.by/news.php");
                response.EnsureSuccessStatusCode();
                var htmlContent = await response.Content.ReadAsStringAsync();
                var articleIds = new List<int>();
                var regex = new Regex(@"news\.php\?readmore=(\d+)", RegexOptions.Compiled);
                foreach (Match match in regex.Matches(htmlContent))
                {
                    if (int.TryParse(match.Groups[1].Value, out int id))
                    {
                        articleIds.Add(id);
                    }
                }

                return Ok(articleIds.Distinct().ToList());
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching news from krmz.by/news.php");
                return StatusCode(500, "Failed to load news list from external source.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching news list.");
                return StatusCode(500, "An internal server error occurred while fetching news list.");
            }
        }

        [HttpGet("api/krmz-article/{id}")]
        public async Task<IActionResult> GetKrmzArticle(int id)
        {
            var client = _httpClientFactory.CreateClient();
            string articleUrl = $"https://www.krmz.by/news.php?readmore={id}";
            _logger.LogInformation($"Attempting to fetch article: {articleUrl}");

            try
            {
                var response = await client.GetAsync(articleUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Article with ID {id} not found or inaccessible. Status: {response.StatusCode}");
                    return NotFound($"Article with ID {id} not found.");
                }

                var htmlContent = await response.Content.ReadAsStringAsync();
                return Content(htmlContent, "text/html", Encoding.UTF8);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error fetching article {id} from {articleUrl}");
                return StatusCode(500, $"Failed to load article {id} from external source. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while fetching article {id}.");
                return StatusCode(500, $"An internal server error occurred while fetching article {id}.");
            }
        }
    }
}