using ConvicartWebApp.Interface;
using ConvicartWebApp.Models;

namespace ConvicartWebApp.Services
{
    public class AddressService : IAddressService
    {
        private readonly ConvicartWarehouseContext _context;

        public AddressService(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public async Task SaveOrUpdateAddressAsync(int customerId, Address address)
        {
            // Check if the address already exists (update)
            var existingAddress = await _context.Addresses.FindAsync(customerId);
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
                // Create a new address and assign the CustomerId to AddressId
                address.AddressId = customerId;
                await _context.Addresses.AddAsync(address);
            }

            // Save changes to address
            await _context.SaveChangesAsync();

            // Update Customer with AddressId
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null)
            {
                customer.AddressId = address.AddressId; // Set the AddressId in Customer
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync(); // Save changes to Customer
            }
        }
    }

}
