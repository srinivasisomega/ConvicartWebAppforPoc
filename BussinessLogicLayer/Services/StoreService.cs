using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _repository;

        public StoreService(IStoreRepository repository)
        {
            _repository = repository;
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
            var items = _repository.GetAllStores();

            if (!string.IsNullOrEmpty(searchTerm))
                items = items.Where(i => i.ProductName.Contains(searchTerm));

            if (preferences != null && preferences.Any())
                items = items.Where(i => preferences.Contains(i.Preference.PreferenceName));

            if (difficulty != null && difficulty.Any())
                items = items.Where(i => difficulty.Contains(i.Difficulty));

            if (cookTimeMin.HasValue)
                items = items.Where(i => i.CookTime >= TimeSpan.FromMinutes(cookTimeMin.Value));

            if (cookTimeMax.HasValue)
                items = items.Where(i => i.CookTime <= TimeSpan.FromMinutes(cookTimeMax.Value));

            if (minPoints.HasValue)
                items = items.Where(i => i.Price >= minPoints.Value);

            if (maxPoints.HasValue)
                items = items.Where(i => i.Price <= maxPoints.Value);

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
            return await _repository.GetStoreByIdAsync(id);
        }
    }
}
