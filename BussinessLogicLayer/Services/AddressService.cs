using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using ConvicartWebApp.PresentationLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class AddressService : IAddressService
    {
        private readonly ConvicartWarehouseContext _context;

        public AddressService(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public async Task SaveOrUpdateAddressAsync(int customerId, AddressViewModel viewModel)
        {
            // Check if the customer exists in the database
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                throw new Exception("Customer not found.");
            }

            // Check if the address already exists for this customer (update case)
            var existingAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == customer.AddressId);

            if (existingAddress != null)
            {
                // Update the existing address with the new details from the view model
                existingAddress.StreetAddress = viewModel.StreetAddress;
                existingAddress.City = viewModel.City;
                existingAddress.State = viewModel.State;
                existingAddress.PostalCode = viewModel.PostalCode;
                existingAddress.Country = viewModel.Country;

                _context.Addresses.Update(existingAddress);
            }
            else
            {
                // Create a new Address entity from the view model data
                var newAddress = new Address
                {
                    AddressId = customerId, // Assuming AddressId is unique per customer
                    StreetAddress = viewModel.StreetAddress,
                    City = viewModel.City,
                    State = viewModel.State,
                    PostalCode = viewModel.PostalCode,
                    Country = viewModel.Country
                };

                // Add the new address to the database
                await _context.Addresses.AddAsync(newAddress);

                // Update the customer's AddressId to link the new address
                customer.AddressId = newAddress.AddressId;
            }

            // Save changes to the database for both address and customer entities
            await _context.SaveChangesAsync();
        }
    }
}