using Microsoft.AspNetCore.Mvc;

namespace ConvicartWebApp.Controllers
{
    using ConvicartWebApp.Filter;
    using ConvicartWebApp.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    [TypeFilter(typeof(CustomerInfoFilter))]
    public class PointsController : Controller
    {
        private readonly ConvicartWarehouseContext _context;

        public PointsController(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        // Action for displaying the purchase points form
        public IActionResult DisplayPurchaseForm()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("SignUp", "Customer"); // Redirect to sign-up if customerId is not found in session
            }

            // Initialize the view model
            var model = new PurchasePointsViewModel
            {
                CustomerId = (int)customerId
            };

            return View(model);
        }

        // POST action to handle point purchases
        [HttpPost]
        public IActionResult ProcessPointPurchase(PurchasePointsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("DisplayPurchaseForm", model); // Return to form with validation errors
            }

            // Example currency rate (could be retrieved dynamically)
            decimal pointsToCurrencyRate = 20; // 1 point = 0.10 currency unit

            // Calculate total cost for the points
            model.AmountToPay = model.PointsToPurchase * pointsToCurrencyRate;

            // Retrieve customer details from the database
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == model.CustomerId);

            if (customer == null)
            {
                ModelState.AddModelError("", "Customer not found.");
                return View("DisplayPurchaseForm", model); // Return to form with error
            }

            // Update the customer's point balance
            customer.PointBalance += model.PointsToPurchase;

            // Save changes to the database
            _context.SaveChanges();

            // Optional: Add a confirmation message
            model.ConfirmationMessage = $"Successfully purchased {model.PointsToPurchase} points!";

            return View("DisplayPurchaseForm", model); // Return form with confirmation
        }
    }
}
