using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<Customer?> GetCustomerByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<Customer>> GetCustomersBySubscriptionAsync(string subscription);
        Task<IEnumerable<Customer>> GetCustomersWithExpiringSubscriptionsAsync(DateTime expirationDate);
        Task<int> GetCustomerPointsBalanceAsync(int customerId);
    }

}
