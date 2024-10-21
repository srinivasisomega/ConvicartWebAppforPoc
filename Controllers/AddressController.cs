using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Models;
using ConvicartWebApp.Filter;
namespace ConvicartWebApp.Controllers
{
    /// <summary>
    /// Controller for managing customer addresses.
    /// </summary>
    [TypeFilter(typeof(CustomerInfoFilter))]
    public class AddressController : Controller
    {
        private readonly ConvicartWarehouseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressController"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing data.</param>
        public AddressController(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the view for creating or updating an address.
        /// </summary>
        /// <returns>The view for creating or updating an address.</returns>
        // GET: Address/CreateOrUpdate
        public IActionResult CreateOrUpdateAddress()
        {
            return View();
        }

        /// <summary>
        /// Saves a customer address, either creating a new one or updating an existing address.
        /// </summary>
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

            // Set AddressId to CustomerId
            address.AddressId = customerId.Value;

            // Check if the address already exists (update)
            var existingAddress = await _context.Addresses.FindAsync(customerId.Value);
            if (existingAddress != null)
            {
                // Update existing address details
                existingAddress.StreetAddress = address.StreetAddress;
                existingAddress.City = address.City;
                existingAddress.State = address.State;
                existingAddress.PostalCode = address.PostalCode;
                existingAddress.Country = address.Country;

                _context.Addresses.Update(existingAddress);
            }
            else
            {
                // Create a new address
                await _context.Addresses.AddAsync(address);
            }

            // Save changes to address
            await _context.SaveChangesAsync();

            // Update Customer with AddressId
            var customer = await _context.Customers.FindAsync(customerId.Value);
            if (customer != null)
            {
                customer.AddressId = address.AddressId; // Set the AddressId in Customer
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync(); // Save changes to Customer
            }

            // Redirect to customer profile page
            return RedirectToAction("Profile", "Customer");
        }
    }
}
