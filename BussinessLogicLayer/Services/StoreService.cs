using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
namespace ConvicartWebApp.BussinessLogicLayer.Services
{

    public class StoreService:IStoreService
    {
        private readonly ConvicartWarehouseContext _context;

        public StoreService(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public IQueryable<Store> GetStores(
            string searchTerm = "",
            List<string> preferences = null,
            List<string> difficulty = null,
            int? cookTimeMin = null,
            int? cookTimeMax = null,
            int? minPoints = null,
            int? maxPoints = null)
        {
            // Include related data and fetch items from database
            var items = _context.Stores.Include(i => i.Preference).AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(i => i.ProductName.Contains(searchTerm));
            }

            // Filter by preferences
            if (preferences != null && preferences.Any())
            {
                items = items.Where(i => preferences.Contains(i.Preference.PreferenceName));
            }

            // Filter by difficulty
            if (difficulty != null && difficulty.Any())
            {
                items = items.Where(i => difficulty.Contains(i.Difficulty));
            }

            // Filter by cook time range
            if (cookTimeMin.HasValue)
            {
                var minimumCookTime = TimeSpan.FromMinutes(cookTimeMin.Value);
                items = items.Where(i => i.CookTime >= minimumCookTime);
            }
            if (cookTimeMax.HasValue)
            {
                var maximumCookTime = TimeSpan.FromMinutes(cookTimeMax.Value);
                items = items.Where(i => i.CookTime <= maximumCookTime);
            }

            // Filter by points range (Price)
            if (minPoints.HasValue)
            {
                items = items.Where(i => i.Price >= minPoints.Value);
            }
            if (maxPoints.HasValue)
            {
                items = items.Where(i => i.Price <= maxPoints.Value);
            }

            return items;
        }

        public IQueryable<Store> SortStores(IQueryable<Store> stores, string sortOrder)
        {
            return sortOrder switch
            {
                "Price ascending" => stores.OrderBy(i => i.Price),
                "Price descending" => stores.OrderByDescending(i => i.Price),
                "Rating" => stores.OrderByDescending(i => i.Rating),
                _ => stores.OrderBy(i => i.ProductId),
            };
        }

        public List<Store> PaginateStores(IQueryable<Store> stores, int page, int pageSize, out int totalPages)
        {
            var totalItems = stores.Count();
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return stores.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        public async Task<Store> GetProductByIdAsync(int id)
        {
            return await _context.Stores
                .FirstOrDefaultAsync(p => p.ProductId == id); // Assuming Store contains Product information
        }

    }

}
