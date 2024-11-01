using ConvicartBusinessLogicLayer.Interface;
using ConvicartWebApp.Filter;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ConvicartDataAccessLayer.Models;

namespace ConvicartWebApp.Controllers
{
    [TypeFilter(typeof(CustomerInfoFilter))]
    public class HomeController : Controller
    {
        private readonly IPreferenceService _preferenceService;
        private readonly IQueryService _queryService;
        private readonly IStoreService _storeService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IPreferenceService preferenceService, IQueryService queryService, IStoreService storeService)
        {
            _preferenceService = preferenceService;
            _queryService = queryService;
            _storeService = storeService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IndexAfterLogin()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp", "Customer");
            }

            var customerPreferences = await _preferenceService.GetCustomerPreferencesAsync(customerId.Value);

            var products = await _storeService.GetProductsByPreferencesAsync(customerPreferences);

            var viewModel = new IndexProductViewModel
            {
                CustomerId = customerId.Value,
                Products = products
            };

            return View(viewModel);
        }

        public IActionResult Recipe()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View(new QuerySubmissionViewModel());
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuery(QuerySubmissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _queryService.SubmitQueryAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View("Contact", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

