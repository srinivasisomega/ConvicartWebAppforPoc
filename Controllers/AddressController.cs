using Microsoft.AspNetCore.Mvc;

namespace ConvicartWebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ConvicartWebApp.Models;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using ConvicartWebApp.Models;

    public class AddressController : Controller
    {
        private readonly ConvicartWarehouseContext _context;

        public AddressController(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        // GET: Address/CreateOrUpdate
        public IActionResult CreateOrUpdateAddress()
        {
            return View();
        }

        // POST: Address/SaveAddress
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddress(Address address)
        {
            // Retrieve CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if session is null
            }

            // Set AddressId to CustomerId
            address.AddressId = customerId.Value;

            // Check if the address already exists (update)
            var existingAddress = await _context.Addresses.FindAsync(customerId.Value);
            if (existingAddress != null)
            {
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

            return RedirectToAction("Profile", "Customer"); // Redirect to customer index or any page you need
        }
    }

}
