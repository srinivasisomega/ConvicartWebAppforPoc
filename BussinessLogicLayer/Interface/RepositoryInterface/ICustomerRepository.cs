using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerWithAddressByIdAsync(int customerId);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<Customer?> GetCustomerByEmailAndPasswordAsync(string email, string password);
    }


}
