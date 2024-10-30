using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<Address?> GetAddressByCustomerIdAsync(int customerId);
    }

}
