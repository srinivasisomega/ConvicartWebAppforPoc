using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Filter;
using ConvicartBusinessLogicLayer.Interface;
using ConvicartWebApp.ViewModels;
using ConvicartBusinessLogicLayer.Dto;
namespace ConvicartWebApp.Controllers
{
    /// <summary>
    /// Controller for managing customer addresses.
    /// </summary>
    [TypeFilter(typeof(CustomerInfoFilter))]
    [SessionAuthorize]
    public class AddressController : Controller
    {
        private readonly IAddressService AddressService;

        /// Initializes a new instance of the <see cref="AddressController"/> class.
        /// <param name="addressService">The service for managing addresses.</param>
        public AddressController(IAddressService addressService)
        {
            AddressService = addressService;
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
        public async Task<IActionResult> SaveAddress(AddressViewModel viewModel, int? customerId)
        {
            if (customerId == null)
            {
                return BadRequest("Customer ID is required.");
            }

            // Map AddressViewModel to AddressDTO
            var addressDto = new AddressDTO
            {
                StreetAddress = viewModel.StreetAddress,
                City = viewModel.City,
                State = viewModel.State,
                PostalCode = viewModel.PostalCode,
                Country = viewModel.Country
            };

            // Delegate the save/update operation to the AddressService using the AddressDTO
            await AddressService.SaveOrUpdateAddressAsync(customerId.Value, addressDto);

            // Redirect to customer profile page
            return RedirectToAction("Profile", "Customer");
        }
    }
}

