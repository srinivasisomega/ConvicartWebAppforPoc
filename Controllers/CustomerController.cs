using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Models;
using Microsoft.EntityFrameworkCore;
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

                TempData["CustomerId"] = customer.CustomerId;

                return RedirectToAction("Subscription");
            }

            return View(customer);
        }

        // GET: Subscription Page
        public async Task<IActionResult> Subscription()
        {
            if (TempData["CustomerId"] is int customerId)
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null) return NotFound();

                return View(customer);
            }

            return RedirectToAction("SignUp");
        }

        // POST: Handle Subscription Update
        [HttpPost]
        public async Task<IActionResult> UpdateSubscription(int customerId, string subscriptionType)
        {
            if (subscriptionType != "Bronze" && subscriptionType != "Silver" && subscriptionType != "Gold")
            {
                ModelState.AddModelError("", "Invalid subscription type.");
                return RedirectToAction("Subscription");
            }

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return NotFound();

            customer.Subscription = subscriptionType;
            customer.SubscriptionDate = DateTime.Now;

            _context.Update(customer);
            await _context.SaveChangesAsync();

            // Store customerId in TempData to pass it to Preferences action
            TempData["CustomerId"] = customer.CustomerId;

            return RedirectToAction("Preferences");
        }
        // GET: Preferences Page
        public async Task<IActionResult> Preferences()
        {
            // Retrieve customerId from TempData
            if (TempData["CustomerId"] is int customerId)
            {
                var preferences = await _context.Preferences.ToListAsync();
                ViewBag.CustomerId = customerId; // Pass customerId to view
                return View(preferences);
            }

            return RedirectToAction("Subscription"); // Redirect back if customerId is not available
        }
        [HttpPost]
        public async Task<IActionResult> SavePreferences(int customerId, List<int> selectedPreferences)
        {
            // Validate if customerId is valid
            if (customerId <= 0)
            {
                ModelState.AddModelError("", "Invalid customer ID.");
                return RedirectToAction("Preferences");
            }

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return NotFound();

            // Clear existing preferences
            var existingPreferences = _context.CustomerPreferences.Where(cp => cp.CustomerId == customerId);
            _context.CustomerPreferences.RemoveRange(existingPreferences);

            // Check if any preferences were selected
            if (selectedPreferences != null && selectedPreferences.Any())
            {
                // Add new preferences
                foreach (var preferenceId in selectedPreferences)
                {
                    var customerPreference = new CustomerPreference
                    {
                        CustomerId = customerId,
                        PreferenceId = preferenceId
                    };
                    _context.CustomerPreferences.Add(customerPreference);
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "Customer", new { customerId = customer.CustomerId });

        }


        // GET: Confirmation Page
        public async Task<IActionResult> Profile(int customerId)
        {
            // Fetch customer details
            var customer = await _context.Customers
                .Include(c => c.Address) // Include Address relationship
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null) return NotFound();

            // Fetch preferences selected by the customer
            var customerPreferences = await _context.CustomerPreferences
                .Where(cp => cp.CustomerId == customerId)
                .Include(cp => cp.Preference)
                .ToListAsync();

            // Create and populate the ViewModel
            var viewModel = new CustomerProfileViewModel
            {
                Customer = customer,
                Address = customer.Address,
                CustomerPreferences = customerPreferences
            };

            // Pass ViewModel to the view
            return View(viewModel);
        }


    }
}

