using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Models;

namespace ConvicartWebApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ConvicartWarehouseContext _context;

        public CustomerController(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        // GET: SignUp Page
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Handle SignUp
        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("Name,Number,Email,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();

                // After successful sign-up, redirect to the "Subscription" page
                return RedirectToAction("Subscription", "Customer");
            }

            // If validation fails, stay on the SignUp page
            return View(customer);
        }

        // GET: Subscription Page
        public IActionResult Subscription()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Subcription([Bind("Subscription,SubscriptionDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();

                // After successful sign-up, redirect to the "Subscription" page
                return RedirectToAction("Preferences", "Customer");
            }

            // If validation fails, stay on the SignUp page
            return View(customer);
        }
    }
}
