using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    // CustomerService.cs
    public class CustomerService : ICustomerService
    {
        private readonly ConvicartWarehouseContext _context;

        public CustomerService(ConvicartWarehouseContext context)
        {
            _context = context;
        }
        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }
        public async Task<Customer> GetCustomerWithAddressByIdAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }
        public async Task UpdateCustomerProfileAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers.FindAsync(customer.CustomerId);
            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.Number = customer.Number;
                existingCustomer.Email = customer.Email;
                existingCustomer.Age = customer.Age;
                existingCustomer.Gender = customer.Gender;

                await _context.SaveChangesAsync();
            }
        }
        public Customer GetCustomerById(int? customerId)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public void AddPointsToCustomer(Customer customer, int points)
        {
            customer.PointBalance += points;
            _context.SaveChanges();
        }

        public async Task<Customer> GetCustomerByEmailAndPasswordAsync(string email, string password)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password)
                .ConfigureAwait(false);
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email)
                .ConfigureAwait(false);
        }

        public bool VerifyPassword(Customer? customer, string currentPassword)
        {
            return customer.Password == currentPassword;
        }

        public async Task<bool> ChangePasswordAsync(Customer? customer, string newPassword)
        {
            customer.Password = newPassword;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SaveProfileImageAsync(IFormFile image, int customerId)
        {
            var customer = GetCustomerById(customerId);

            if (customer == null || image == null || image.Length == 0)
            {
                return false; // Invalid customer or image
            }

            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                customer.ProfileImage = memoryStream.ToArray();
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
