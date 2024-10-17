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

        

        // POST: Handle SignIn
        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            // Check if the model state is valid
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            // Find the customer by email and password
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password)
                .ConfigureAwait(false);

            if (customer == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            // Store CustomerId in session
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);

            return RedirectToAction("Profile", new { customerId = customer.CustomerId });
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
                await _context.SaveChangesAsync().ConfigureAwait(false);

                // Store CustomerId in session
                HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);

                return RedirectToAction("Subscription");
            }

            return View(customer);
        }

        // GET: Subscription Page
        public async Task<IActionResult> Subscription()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp");
            }

            var customer = await _context.Customers.FindAsync(customerId).ConfigureAwait(false);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // POST: Handle Subscription Update
        [HttpPost]
        public async Task<IActionResult> UpdateSubscription(string subscriptionType)
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp");
            }

            if (!new[] { "Bronze", "Silver", "Gold" }.Contains(subscriptionType))
            {
                ModelState.AddModelError("", "Invalid subscription type.");
                return RedirectToAction("Subscription");
            }

            var customer = await _context.Customers.FindAsync(customerId).ConfigureAwait(false);
            if (customer == null) return NotFound();

            customer.Subscription = subscriptionType;
            customer.SubscriptionDate = DateTime.Now;

            _context.Update(customer);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction("Preferences");
        }

        // GET: Preferences Page
        public async Task<IActionResult> Preferences()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Subscription");
            }

            var preferences = await _context.Preferences.ToListAsync().ConfigureAwait(false);
            ViewBag.CustomerId = customerId; // Pass customerId to view

            return View(preferences);
        }

        // POST: Save Preferences
        [HttpPost]
        public async Task<IActionResult> SavePreferences(List<int> selectedPreferences)
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
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
                        CustomerId = customerId.Value,
                        PreferenceId = preferenceId
                    };
                    _context.CustomerPreferences.Add(customerPreference);
                }
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction("Profile", new { customerId = customer.CustomerId });
        }

        // GET: Profile Page
        public async Task<IActionResult> Profile()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignIn");
            }

            var customer = await _context.Customers
                .Include(c => c.Address) // Include Address relationship
                .FirstOrDefaultAsync(c => c.CustomerId == customerId.Value);

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
        public async Task<IActionResult> UpdatePreference()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Subscription");
            }

            // Get all preferences available in the system
            var preferences = await _context.Preferences.ToListAsync().ConfigureAwait(false);

            // Get the customer's currently selected preferences
            var selectedPreferences = await _context.CustomerPreferences
                .Where(cp => cp.CustomerId == customerId.Value)
                .Select(cp => cp.PreferenceId)
                .ToListAsync();

            ViewBag.CustomerId = customerId;
            ViewBag.SelectedPreferences = selectedPreferences; // Pass selected preferences to the view

            return View(preferences);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatedPreferences(List<int> selectedPreferences)
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp");
            }

            // Find the customer by ID
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId.Value)
                .ConfigureAwait(false);

            if (customer == null) return NotFound();

            // Clear existing preferences for the customer
            var existingPreferences = await _context.CustomerPreferences
                .Where(cp => cp.CustomerId == customer.CustomerId)
                .ToListAsync()
                .ConfigureAwait(false);
            _context.CustomerPreferences.RemoveRange(existingPreferences);

            // Add the newly selected preferences
            if (selectedPreferences != null && selectedPreferences.Any())
            {
                foreach (var preferenceId in selectedPreferences)
                {
                    var customerPreference = new CustomerPreference
                    {
                        CustomerId = customer.CustomerId,
                        PreferenceId = preferenceId
                    };
                    _context.CustomerPreferences.Add(customerPreference);
                }
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction("Profile", new { customerId = customer.CustomerId });
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            // Check if CustomerId exists in session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                // Redirect to SignUp if CustomerId is not found
                return RedirectToAction("SignUp", "Customer");
            }

            // Retrieve the customer data using CustomerId from session
            var customer = _context.Customers.Find(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfileSave(Customer model)
        {
            
                var customer = _context.Customers.Find(model.CustomerId);

                if (customer != null)
                {
                    customer.Name = model.Name;
                    customer.Number = model.Number;
                    customer.Email = model.Email;
                    customer.Age = model.Age;
                    customer.Gender = model.Gender;

                    _context.SaveChanges();
                    return RedirectToAction("Profile", new { customerId = model.CustomerId });
                }
           

            return View(model);
        }
        // GET: Upload Profile Image
        [HttpGet]
        public IActionResult UploadProfileImage()
        {
            return View();
        }

        
        
        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(int id, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    memoryStream.Position = 0; // Reset position

                    var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
                    if (customer != null)
                    {
                        // Convert uploaded image to byte[] and store in database
                        customer.ProfileImage = memoryStream.ToArray();
                        _context.Update(customer);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound(); // Handle case where customer is not found
                    }
                }
            }
            return RedirectToAction("ProfilePic", new { id });
        }


        // GET: Get Profile Image to display in view
        public IActionResult GetProfileImage(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer?.ProfileImage != null)
            {
                return File(customer.ProfileImage, "image/jpeg"); // Assuming image type is jpeg
            }
            else
            {
                return NotFound();  // Handle case where no image exists
            }
        }
    }

}


