using ConvicartWebApp.Models;

namespace ConvicartWebApp.Interface
{
    public interface IAddressService
    {
        Task SaveOrUpdateAddressAsync(int customerId, Address address);
    }

}
