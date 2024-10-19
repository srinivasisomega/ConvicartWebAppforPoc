using ConvicartWebApp.Filter;
using ConvicartWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ConvicartWebApp.Controllers
{
    [TypeFilter(typeof(CustomerInfoFilter))]

    public class HomeController : Controller
    {
        private readonly ConvicartWarehouseContext _context;
        private readonly ILogger<HomeController> _logger;

        // Combined constructor for both _context and _logger
        public HomeController(ConvicartWarehouseContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        

        public IActionResult Recipe()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Subscription()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
       

        // Action to handle the submission of the Query form
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Mobile,Description")] QuerySubmission query1)
        {
            if (ModelState.IsValid)
            {
                _context.Add(query1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirect to Index on success
            }
            return View("Contact.cshtml"); // Return to the Contact view if ModelState is invalid
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

