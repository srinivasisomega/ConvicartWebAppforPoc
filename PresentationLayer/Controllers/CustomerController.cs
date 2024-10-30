using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Filter;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
namespace ConvicartWebApp.PresentationLayer.Controllers
{
    /// <summary>
    /// This controller is responsible for signup, signin, supscription selection, preference selection, viewing profile page, updating profilepage,
    /// uploading profile photo, displaying profile photo, logout, forgot password
    /// </summary>
    [TypeFilter(typeof(CustomerInfoFilter))]
    public class CustomerController : Controller
    {
        private readonly ConvicartWarehouseContext _context;
        private readonly ICustomerService CustomerService;
        private readonly IPreferenceService PreferenceService;
        private readonly IPasswordResetService PasswordResetService;
        private readonly ISubscriptionService SubscriptionService;
        public CustomerController(ConvicartWarehouseContext context, ISubscriptionService subscriptionService, ICustomerService customerService, IPreferenceService preferenceService, IPasswordResetService passwordResetService)
        {
            _context = context;
            CustomerService = customerService;
            PreferenceService = preferenceService;
            PasswordResetService = passwordResetService;
            SubscriptionService = subscriptionService;
        }

        // POST: Handle SignIn by checking if there are matching email and password in the customer table, if
        //they match customer id is saved in session
        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            // Check if the model state is valid
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewData["SignInErrors"] = new List<string> { "Email and password are required." };
                return View("SignUp");
            }

            // Find the customer by email and password
            var customer = await CustomerService.GetCustomerByEmailAndPasswordAsync(email, password);

            if (customer == null)
            {
                ViewData["SignInErrors"] = new List<string> { "Invalid email or password." };
                return View("SignUp");
            }

            // Store CustomerId in session
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);

            return RedirectToAction("Profile", new { customerId = customer.CustomerId });
        }

        // GET: Renders the Signup page and sigin partial views
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("Name,Number,Email,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingCustomer = await CustomerService.GetCustomerByEmailAsync(customer.Email);


                if (existingCustomer != null)
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(customer); // Return the view with an error message
                }

                _context.Add(customer);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                // Store CustomerId in session
                HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);

                return RedirectToAction("Subscription");
            }

            return View(customer);
        }
        [HttpGet]
        public IActionResult SignInWithGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded) return RedirectToAction("SignUp");

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                ViewData["SignInErrors"] = new List<string> { "Google login failed. Please try again." };
                return RedirectToAction("SignUp");
            }

            // Check if user already exists using LINQ
            var customer = await CustomerService.GetCustomerByEmailAsync(email);

            if (customer == null)
            {
                // New user, create a Customer entry
                customer = new Customer
                {
                    Name = name,
                    Number = "Change number",
                    Email = email,
                    Password = "Change Password",
                    PointBalance = 0,
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            // Store CustomerId in session
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);

            return RedirectToAction("Profile", new { customerId = customer.CustomerId });
        }



        // GET: checks if there is customer id in session if yes sends the customer's data to view for selection the type of subscription.
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        [SessionAuthorize]
        public async Task<IActionResult> Subscription(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId).ConfigureAwait(false);
            if (customer == null) return NotFound();

            return View(customer);
        }

        //this method is responsible for add no of days to subscription date, adding points to point balence, and adding subscription to the customers table.
        [HttpPost]
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        [SessionAuthorize]
        public IActionResult UpdateSubscription(string subscriptionType, int days, decimal amount, int customerId)
        {
            if (!SubscriptionService.UpdateSubscription(subscriptionType, days, amount, customerId, out string errorMessage))
            {
                // Add error message to ModelState and return to Subscription view if there's an issue
                ModelState.AddModelError("", errorMessage);
                return RedirectToAction("Subscription", new { customerId = customerId });
            }

            // Check if a record with the same customerId exists in the CustomerPreferences table
            bool hasPreferences = _context.CustomerPreferences.Any(p => p.CustomerId == customerId);

            // Redirect based on whether preferences already exist
            if (hasPreferences)
            {
                return RedirectToAction("Profile");
            }
            else
            {
                return RedirectToAction("Preferences");
            }
        }


        // GET: Preferences Page  checks if there is customer id in session if yes sends the customer's data to view for selection of preferences.
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        [SessionAuthorize]
        public async Task<IActionResult> Preferences(int customerId)
        {

            var preferences = await PreferenceService.GetPreferencesAsync().ConfigureAwait(false);
            ViewBag.CustomerId = customerId;

            return View(preferences);
        }

        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> SavePreferences(List<int> selectedPreferences)
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");

            await PreferenceService.UpdateCustomerPreferencesAsync(customerId.Value, selectedPreferences).ConfigureAwait(false);

            return RedirectToAction("Profile", new { customerId });
        }

        // GET: Profile Page this page displays the address, preferences, pointbalence, subscription and other importent details related to customer
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        [SessionAuthorize]
        public async Task<IActionResult> Profile(int? customerId)
        {
            if (!customerId.HasValue) return BadRequest();

            var customer = await CustomerService.GetCustomerWithAddressByIdAsync(customerId.Value);
            if (customer == null) return NotFound();

            var customerPreferences = await PreferenceService.GetCustomerPreferencesListAsync(customerId.Value);

            var viewModel = new CustomerProfileViewModel
            {
                Customer = customer,
                Address = customer.Address,
                CustomerPreferences = customerPreferences
            };

            return View(viewModel);
        }

        // it is responsible for rendering a view that show existing preferences slected by customer and show all other preferences from preferences table.
        [SessionAuthorize]
        public async Task<IActionResult> UpdatePreference()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Subscription");
            }

            var preferences = await PreferenceService.GetPreferencesAsync().ConfigureAwait(false);
            var selectedPreferences = await PreferenceService.GetCustomerPreferencesAsync(customerId.Value).ConfigureAwait(false);

            ViewBag.CustomerId = customerId;
            ViewBag.SelectedPreferences = selectedPreferences;

            return View(preferences);
        }

        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> UpdatedPreferences(List<int> selectedPreferences)
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            await PreferenceService.UpdateCustomerPreferencesAsync(customerId.Value, selectedPreferences).ConfigureAwait(false);

            return RedirectToAction("Profile", new { customerId });
        }

        [HttpGet]
        [SessionAuthorize]
        public IActionResult EditProfile()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            var customer = CustomerService.GetCustomerByIdAsync(customerId.Value).Result;
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileSave(Customer model)
        {
            await CustomerService.UpdateCustomerProfileAsync(model);
            return RedirectToAction("Profile", new { customerId = model.CustomerId });
        }

        //renders a page to upload the image.
        [HttpGet]
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        [SessionAuthorize]
        public IActionResult UploadProfileImage(int? customerId)
        {
            var customer = CustomerService.GetCustomerById(customerId);
            if (customer == null)
            {
                return NotFound();  // Handle case where customer is not found
            }

            return View(customer);  // Return the customer model to the view
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        [SessionAuthorize]
        public async Task<IActionResult> UploadProfileImageSave(IFormFile image, int? customerId)
        {
            if (customerId == null)
            {
                return NotFound();  // Invalid customer ID
            }

            var isSaved = await CustomerService.SaveProfileImageAsync(image, customerId.Value);

            if (!isSaved)
            {
                return NotFound();  // Handle case where customer or image is invalid
            }

            return RedirectToAction("Profile", new { id = customerId });
        }
        [SessionAuthorize]
        public IActionResult GetProfileImage(int id)
        {
            var customer = CustomerService.GetCustomerById(id);
            if (customer?.ProfileImage != null)
            {
                return File(customer.ProfileImage, "image/jpeg");  // Serve image
            }
            else
            {
                return NotFound();  // No image found
            }
        }
        private static Dictionary<string, string> resetCodes = new Dictionary<string, string>();

        // GET: Customer/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Customer/ForgotPassword
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("Email", "Email is required.");
                return View();
            }

            // Use the service to generate and send the reset code
            PasswordResetService.GenerateAndSendResetCode(email);

            TempData["Message"] = "If the email exists, a reset code has been sent.";
            TempData["Email"] = email; // Store email in TempData
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        // GET: Customer/ForgotPasswordConfirmation
        public IActionResult ForgotPasswordConfirmation()
        {
            // Re-store and keep TempData for the email
            TempData.Keep("Email");
            ViewBag.Email = TempData["Email"]; // Display email for verification if needed
            return View();
        }

        // GET: Customer/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword()
        {
            // Retrieve email from TempData
            var email = TempData["Email"] as string;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            // Re-store email in TempData and keep it for subsequent requests
            TempData["Email"] = email;
            TempData.Keep("Email");

            return View(new ResetPasswordModel { Email = email });
        }
        // POST: Customersword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the customer by email using the async method and await the result
            var customer = await CustomerService.GetCustomerByEmailAsync(model.Email);
            if (customer == null)
            {
                ModelState.AddModelError("", "Customer not found.");
                return View(model);
            }

            // Change the customer's password asynchronously and await the call
            await CustomerService.ChangePasswordAsync(customer, model.NewPassword);

            TempData["Message"] = "Your password has been successfully reset.";
            return RedirectToAction("ResetPasswordConfirmation");
        }

        // GET: Customer/ResetPasswordConfirmation
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        [SessionAuthorize]
        public IActionResult ChangePassword(int? customerId)
        {

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        [SessionAuthorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, int? customerId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Use the service to retrieve the customer
            var customer = CustomerService.GetCustomerById(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            // Verify the current password using the service
            if (!CustomerService.VerifyPassword(customer, model.CurrentPassword))
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(model);
            }

            // Update the password using the service
            await CustomerService.ChangePasswordAsync(customer, model.NewPassword);

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }
        public IActionResult Logout()
        {
            // Clear the CustomerId from the session
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        [SessionAuthorize]
        public async Task<IActionResult> GetSubscription(int customerId)
        {
            var viewModel = await SubscriptionService.GetCustomerSubscriptionAsync(customerId);

            if (viewModel == null)
                return NotFound();

            return View("SubscriptionUpdate", viewModel);
        }



    }

}


