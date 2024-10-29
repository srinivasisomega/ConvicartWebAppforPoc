using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
namespace ConvicartWebApp.DataAccessLayer.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly ConvicartWarehouseContext _context;

        public StoreRepository(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public IQueryable<Store> GetAllStores()
        {
            return _context.Stores.Include(i => i.Preference).AsQueryable();
        }

        public async Task<Store> GetStoreByIdAsync(int id)
        {
            return await _context.Stores.FirstOrDefaultAsync(p => p.ProductId == id);
        }
    }
}
