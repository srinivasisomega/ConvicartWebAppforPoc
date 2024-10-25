using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
namespace ConvicartWebApp.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly ConvicartWarehouseContext _context;

        public CustomerRepository(ConvicartWarehouseContext context) : base(context)
        {
            _context = context;
        }

        // Get customer by email
        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        // Get customer by phone number
        public async Task<Customer?> GetCustomerByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Number == phoneNumber);
        }

        // Get customers by subscription type
        public async Task<IEnumerable<Customer>> GetCustomersBySubscriptionAsync(string subscription)
        {
            return await _context.Customers
                                 .Where(c => c.Subscription == subscription)
                                 .ToListAsync();
        }

        // Get customers with expiring subscriptions
        public async Task<IEnumerable<Customer>> GetCustomersWithExpiringSubscriptionsAsync(DateTime expirationDate)
        {
            return await _context.Customers
                                 .Where(c => c.SubscriptionDate.HasValue && c.SubscriptionDate.Value <= expirationDate)
                                 .ToListAsync();
        }

        // Get customer's points balance
        public async Task<int> GetCustomerPointsBalanceAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            return customer?.PointBalance ?? 0;
        }
    }

}
