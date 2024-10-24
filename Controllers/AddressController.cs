using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Models;
using ConvicartWebApp.Filter;
using ConvicartWebApp.Interface;

namespace ConvicartWebApp.Controllers
{
    /// <summary>
    /// Controller for managing customer addresses.
    /// </summary>
    [TypeFilter(typeof(CustomerInfoFilter))]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;

        /// Initializes a new instance of the <see cref="AddressController"/> class.
        /// <param name="addressService">The service for managing addresses.</param>
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        /// Displays the view for creating or updating an address.
        /// <returns>The view for creating or updating an address.</returns>
        // GET: Address/CreateOrUpdate
        public IActionResult CreateOrUpdateAddress()
        {
            return View();
        }

        /// Saves a customer address, either creating a new one or updating an existing address.
        /// <param name="address">The address information to save.</param>
        /// <returns>A redirect to the customer's profile page.</returns>
        // POST: Address/SaveAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddress(Address address)
        {
            // Retrieve CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                // Redirect to login if session is null
                return RedirectToAction("Login", "Account");
            }

            // Delegate the save/update operation to the AddressService
            await _addressService.SaveOrUpdateAddressAsync(customerId.Value, address);

            // Redirect to customer profile page
            return RedirectToAction("Profile", "Customer");
        }
    }
}
