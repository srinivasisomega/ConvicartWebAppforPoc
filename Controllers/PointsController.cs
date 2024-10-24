using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Filter;
using ConvicartWebApp.Models;
using ConvicartWebApp.Interface;
using ConvicartWebApp.Services;
using System.Linq;

namespace ConvicartWebApp.Controllers
{
    [TypeFilter(typeof(CustomerInfoFilter))]
    public class PointsController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IPointsService _pointsService;

        // Dependency Injection through constructor
        public PointsController(ICustomerService customerService, IPointsService pointsService)
        {
            _customerService = customerService;
            _pointsService = pointsService;
        }

        // Action for displaying the purchase points form
        public IActionResult DisplayPurchaseForm()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("SignUp", "Customer");
            }

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

            try
            {
                // Delegate the point purchase and payment logic to the PointsService
                var amountToPay = _pointsService.CalculateAmountToPay(model.PointsToPurchase);
                var customer = _customerService.GetCustomerById(model.CustomerId);

                if (customer == null)
                {
                    ModelState.AddModelError("", "Customer not found.");
                    return View("DisplayPurchaseForm", model); // Return to form with error
                }

                // Update points and save through the customer service
                _customerService.AddPointsToCustomer(customer, model.PointsToPurchase);
                model.AmountToPay = amountToPay;

                // Confirmation message
                model.ConfirmationMessage = $"Successfully purchased {model.PointsToPurchase} points!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("DisplayPurchaseForm", model); // Handle errors gracefully
            }

            return View("DisplayPurchaseForm", model); // Return form with confirmation
        }
    }
}
