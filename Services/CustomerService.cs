using ConvicartWebApp.Interface;
using ConvicartWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ConvicartWebApp.Services
{
    // CustomerService.cs
    public class CustomerService : ICustomerService
    {
        private readonly ConvicartWarehouseContext _context;

        public CustomerService(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public Customer GetCustomerById(int customerId)
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

    }
}
