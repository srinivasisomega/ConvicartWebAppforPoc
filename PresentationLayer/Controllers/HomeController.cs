using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using ConvicartWebApp.Filter;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ConvicartWebApp.PresentationLayer.Controllers
{
    [TypeFilter(typeof(CustomerInfoFilter))]

    public class HomeController : Controller
    {
        private readonly ConvicartWarehouseContext _context;
        private readonly IPreferenceService PreferenceService;
        private readonly ILogger<HomeController> _logger;

        // Combined constructor for both _context and _logger
        public HomeController(ConvicartWarehouseContext context, ILogger<HomeController> logger, IPreferenceService preferenceService)
        {
            _context = context;
            PreferenceService = preferenceService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Indexafterlogin()
        {
            // Check if the customer is logged in
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                // If no customer session, redirect or return empty result
                return RedirectToAction("SignUp", "Customer");  // You can modify this as per your login logic
            }

            // Get CustomerId from session
            int customerId = HttpContext.Session.GetInt32("CustomerId").Value;

            // Fetch preferences of the logged-in customer
            var customerPreferences = _context.CustomerPreferences
                .Where(cp => cp.CustomerId == customerId)
                .Select(cp => cp.PreferenceId)
                .ToList();

            // Fetch products that match any of the customer's preferences
            var products = _context.Stores
                .Where(p => customerPreferences.Contains(p.PreferenceId ?? 0))
                .ToList();

            // Pass the products to the view
            return View(products);
        }

        public IActionResult Recipe()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        

        public IActionResult About()
        {
            return View();
        }


        // Action to handle the submission of the Query form
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Mobile,Email,Description")] QuerySubmission querySubmission)
        {
            if (ModelState.IsValid)
            {
                _context.Add(querySubmission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(querySubmission);
        }

        //[HttpPost]
        //public async Task<IActionResult> SignUp([Bind("Name,Number,Email,Password")] Customer customer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(customer);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index)); // Redirect to Index on success
        //    }
        //    return View("Subscription"); // Return to the Contact view if ModelState is invalid
        //}
        //[HttpPost]
        //public async Task<IActionResult> Signin([Bind("Email,Password")] Customer customer)
        //{
        //    return View();
        //}

        //    public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

