using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using ConvicartWebApp.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ConvicartWebApp.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ConvicartWarehouseContext context) : base(context) { }

        public async Task<Customer?> GetCustomerWithAddressByIdAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetCustomerByEmailAndPasswordAsync(string email, string password)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
        }
    }


}
