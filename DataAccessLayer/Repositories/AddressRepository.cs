using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
namespace ConvicartWebApp.DataAccessLayer.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ConvicartWarehouseContext _context;

        public AddressRepository(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public async Task<Address?> GetAddressByCustomerIdAsync(int customerId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == customerId);
        }

        public async Task AddAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
        }

        public void Update(Address address)
        {
            _context.Addresses.Update(address);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
