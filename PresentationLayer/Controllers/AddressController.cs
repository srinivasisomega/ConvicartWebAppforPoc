using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Filter;
using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ConvicartWebApp.PresentationLayer.Controllers
{
    /// <summary>
    /// Controller for managing customer addresses.
    /// </summary>
    [TypeFilter(typeof(CustomerInfoFilter))]
    [Authorize(Policy = "GoldOnly")]
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
            return View(new AddressViewModel());
        }

        /// Saves a customer address, either creating a new one or updating an existing address.
        /// <param name="viewModel">The address information to save, provided by the view model.</param>
        /// <returns>A redirect to the customer's profile page.</returns>
        // POST: Address/SaveAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        public async Task<IActionResult> SaveAddress(AddressViewModel viewModel,int? customerId)
        {
            // Delegate the save/update operation to the AddressService using the view model
            await _addressService.SaveOrUpdateAddressAsync(customerId.Value, viewModel);
            // Redirect to customer profile page
            return RedirectToAction("Profile", "Customer");
        }
    }
}
