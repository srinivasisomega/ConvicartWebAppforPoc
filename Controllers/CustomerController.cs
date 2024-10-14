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
                await _context.SaveChangesAsync().ConfigureAwait(false); // Use ConfigureAwait(false)

                TempData["CustomerId"] = customer.Id;

                return RedirectToAction("Subscription");
            }

            return View(customer);
        }

        // GET: Subscription Page
        public async Task<IActionResult> Subscription()
        {
            if (TempData["CustomerId"] is int customerId)
            {
                var customer = await _context.Customers.FindAsync(customerId).ConfigureAwait(false);
                if (customer == null) return NotFound();

                return View(customer);
            }

            return RedirectToAction("SignUp");
        }

        // POST: Handle Subscription Update
        [HttpPost]
        public async Task<IActionResult> UpdateSubscription(int customerId, string subscriptionType)
        {
            if (!new[] { "Bronze", "Silver", "Gold" }.Contains(subscriptionType))
            {
                ModelState.AddModelError("", "Invalid subscription type.");
                return RedirectToAction("Subscription");
            }

            var customer = await _context.UserRoles.FindAsync(customerId).ConfigureAwait(false);
            if (customer == null) return NotFound();

            customer.RoleId = subscriptionType;
            customer.SubscriptionStartDate = DateTime.Now;

            _context.Update(customer);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            TempData["CustomerId"] = customer.UserId;

            return RedirectToAction("Preferences");
        }

        // GET: Preferences Page
        public async Task<IActionResult> Preferences()
        {
            if (TempData["CustomerId"] is int customerId)
            {
                var preferences = await _context.Preferences.ToListAsync().ConfigureAwait(false);
                ViewBag.CustomerId = customerId; // Pass customerId to view
                return View(preferences);
            }

            return RedirectToAction("Subscription");
        }

        // POST: Save Preferences
        [HttpPost]
        public async Task<IActionResult> SavePreferences(string customerId, List<int> selectedPreferences)
        {
            if (customerId == null)
            {
                ModelState.AddModelError("", "Invalid customer ID.");
                return RedirectToAction("Preferences");
            }

            var customer = await _context.Customers.FindAsync(customerId).ConfigureAwait(false);
            if (customer == null) return NotFound();

            // Clear existing preferences
            var existingPreferences = await _context.CustomerPreferences
                .Where(cp => cp.CustomerId == customerId).ToListAsync().ConfigureAwait(false);
            _context.CustomerPreferences.RemoveRange(existingPreferences);

            // Add new preferences
            if (selectedPreferences != null && selectedPreferences.Any())
            {
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

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction("Profile", "Customer", new { customerId = customer.Id });
        }

        // GET: Profile Page
        // GET: Confirmation Page
        public async Task<IActionResult> Profile(string customerId)
        {
            // Fetch customer details
            var customer = await _context.Customers
                .Include(c => c.Address) // Include Address relationship
                .FirstOrDefaultAsync(c => c.Id == customerId);

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

            // Dispose of the context before returning the view
            return View(viewModel);
        }

    }
}

