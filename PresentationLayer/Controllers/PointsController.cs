using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Filter;
using ConvicartWebApp.DataAccessLayer.Models;
using ConvicartWebApp.BussinessLogicLayer.Interface;

namespace ConvicartWebApp.PresentationLayer.Controllers
{
    [TypeFilter(typeof(CustomerInfoFilter))]
    [SessionAuthorize]
    public class PointsController : Controller
    {
        private readonly ICustomerService CustomerService;
        private readonly IPointsService PointsService;

        // Dependency Injection through constructor
        public PointsController(ICustomerService customerService, IPointsService pointsService)
        {
            CustomerService = customerService;
            PointsService = pointsService;
        }

        // Action for displaying the purchase points form
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        public IActionResult DisplayPurchaseForm(int? customerId)
        {            

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
                var amountToPay = PointsService.CalculateAmountToPay(model.PointsToPurchase);
                var customer = CustomerService.GetCustomerById(model.CustomerId);

                if (customer == null)
                {
                    ModelState.AddModelError("", "Customer not found.");
                    return View("DisplayPurchaseForm", model); // Return to form with error
                }

                // Update points and save through the customer service
                CustomerService.AddPointsToCustomer(customer, model.PointsToPurchase);
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
