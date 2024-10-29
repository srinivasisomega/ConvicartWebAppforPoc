using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerByIdAsync(int customerId);
        Task<Customer> GetCustomerWithAddressByIdAsync(int customerId);
        Task<Customer> GetCustomerByEmailAsync(string email);
        Task<Customer> GetCustomerByEmailAndPasswordAsync(string email, string password);
        Task UpdateCustomerAsync(Customer customer);
        Task SaveChangesAsync();
    }


}
