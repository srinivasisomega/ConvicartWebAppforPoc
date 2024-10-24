using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Models;
using Microsoft.EntityFrameworkCore;
using ConvicartWebApp.Filter;
using ConvicartWebApp.ViewModels;
using System.Net.Mail;
using ConvicartWebApp.Interface;
namespace ConvicartWebApp.Controllers
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
        public CustomerController(ConvicartWarehouseContext context, ICustomerService customerService)
        {
            _context = context;
            CustomerService = customerService;
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
                return View();
            }

            // Find the customer by email and password
            var customer = await CustomerService.GetCustomerByEmailAndPasswordAsync(email, password);

            if (customer == null)
            {
                ViewData["SignInErrors"] = new List<string> { "Invalid email or password." };
                return View();
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
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == customer.Email);

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


        // GET: checks if there is customer id in session if yes sends the customer's data to view for selection the type of subscription.
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
        //this method is responsible for add no of days to subscription date, adding points to point balence, and adding subscription to the customers table.
        [HttpPost]
        public IActionResult UpdateSubscription(string subscriptionType, int days, decimal amount)
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
            // Fetch the customer from the database using customerId
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            // Calculate the new subscription end date
            DateTime currentDate = DateTime.Now;
            DateTime newSubscriptionEndDate = currentDate.AddDays(days);

            // Calculate points based on the amount
            int points = (int)(amount / 20);

            // Update the customer's subscription and points balance
            customer.Subscription = subscriptionType;
            customer.SubscriptionDate = newSubscriptionEndDate; // Assuming you have a SubscriptionEndDate field
            customer.PointBalance += points; // Assuming customer has a PointBalance property

            // Save changes to the database
            _context.SaveChanges();

            // Return a success message or redirect to a different page
            return RedirectToAction("Preferences");
        }


        // GET: Preferences Page  checks if there is customer id in session if yes sends the customer's data to view for selection of preferences.

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

        // POST: Save Preferences this methods saves the data in customer prefrences table.
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

        // GET: Profile Page this page displays the address, preferences, pointbalence, subscription and other importent details related to customer
        public async Task<IActionResult> Profile()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp");
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
        // it is responsible for rendering a view that show existing preferences slected by customer and show all other preferences from preferences table.
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
        //renders a page that changes name,email,gender,number,age of customer in table.
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

        // the method that updates customer name,email,gender,number,age in edit profile page.
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
        //renders a page to upload the image.
        [HttpGet]
        public IActionResult UploadProfileImage()
        {
            // Retrieve the CustomerId from the session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp", "Customer");  // Redirect if not found
            }

            var customer = _context.Customers.Find(customerId);
            if (customer == null)
            {
                return NotFound();  // Handle case where customer is not found
            }

            return View(customer);  // Return the customer model to the view
        }
        //converts the image into an array to be stored in database.
        [HttpPost]
        public async Task<IActionResult> UploadProfileImageSave(IFormFile image)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp", "Customer");  // Redirect if not found
            }

            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;  // Reset position for reading

                    var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                    if (customer != null)
                    {
                        customer.ProfileImage = memoryStream.ToArray();  // Save image as byte array
                        _context.Update(customer);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound();  // Customer not found
                    }
                }
            }

            return RedirectToAction("Profile", new { id = customerId });
        }
        //converts the array into an image
        public IActionResult GetProfileImage(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer?.ProfileImage != null)
            {
                return File(customer.ProfileImage, "image/jpeg");  // Serve image
            }
            else
            {
                return NotFound();  // No image found
            }
        }
        public IActionResult GetPreferenceImage(int id)
        {
            var customer = _context.Preferences.FirstOrDefault(c => c.PreferenceId == id);
            if (customer?.PreferenceImage != null)
            {
                return File(customer.PreferenceImage, "image/jpeg");  // Serve image
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

            // Generate a 6-digit reset code
            var resetCode = new Random().Next(100000, 999999).ToString();
            resetCodes[email] = resetCode;

            // Send the reset code via email
            SendResetCodeEmail(email, resetCode);

            TempData["Message"] = "If the email exists, a reset code has been sent.";
            TempData["Email"] = email;  // Pass email to the next view
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        private void SendResetCodeEmail(string email, string resetCode)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("convicart6@gmail.com", "vrey nwgz rnln xvzk"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("convicart6@gmail.com"),
                Subject = "Password Reset Code",
                Body = $"Your password reset code is: {resetCode}",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);
            smtpClient.Send(mailMessage);
        }

        // GET: Customer/ForgotPasswordConfirmation
        public IActionResult ForgotPasswordConfirmation()
        {
            ViewBag.Email = TempData["Email"];  // Retrieve email for showing or redirecting
            return View();
        }

        // GET: Customer/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            return View(new ResetPasswordModel { Email = email });
        }

        // POST: Customer/ResetPassword
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate the reset code
            if (!resetCodes.ContainsKey(model.Email) || resetCodes[model.Email] != model.Code)
            {
                ModelState.AddModelError("", "Invalid code.");
                return View(model);
            }

            // Find the customer by email
            var customer = _context.Customers.FirstOrDefault(c => c.Email == model.Email);
            if (customer == null)
            {
                ModelState.AddModelError("", "Customer not found.");
                return View(model);
            }

            // Save the new password as plain text
            customer.Password = model.NewPassword; 
            _context.SaveChanges(); // Save the changes to the database

            resetCodes.Remove(model.Email); // Remove the used code

            TempData["Message"] = "Your password has been successfully reset.";
            return RedirectToAction("ResetPasswordConfirmation");
        }


        // GET: Customer/ResetPasswordConfirmation
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp");
            }
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the customer ID from the session
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp");
            }

            // Find the customer by ID
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            // Check if the current password is correct
            if (customer.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(model);
            }

            // Update the password
            customer.Password = model.NewPassword;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Optionally, redirect or return a success message
            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }

        public IActionResult Logout()
        {
            // Clear the CustomerId from the session
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }
    }

}


