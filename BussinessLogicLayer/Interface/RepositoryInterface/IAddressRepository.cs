using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface
{
    public interface IAddressRepository
    {
        Task<Address?> GetAddressByCustomerIdAsync(int customerId);
        Task AddAsync(Address address);
        void Update(Address address);
        Task SaveChangesAsync();
    }

}
