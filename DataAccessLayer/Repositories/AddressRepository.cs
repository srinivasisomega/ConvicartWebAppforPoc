using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
namespace ConvicartWebApp.DataAccessLayer.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(ConvicartWarehouseContext context) : base(context) { }

        public async Task<Address?> GetAddressByCustomerIdAsync(int customerId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == customerId);
        }
    }


}
