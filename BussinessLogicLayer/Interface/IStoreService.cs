using ConvicartWebApp.DataAccessLayer.Models;
namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    public interface IStoreService
    {
        /// <summary>
        /// Retrieves stores from the database, with optional filtering based on search term, preferences, difficulty, cook time, and price range.
        /// </summary>
        IQueryable<Store> GetStores(
            string searchTerm = "",
            List<string> preferences = null,
            List<string> difficulty = null,
            int? cookTimeMin = null,
            int? cookTimeMax = null,
            int? minPoints = null,
            int? maxPoints = null
        );

        /// <summary>
        /// Sorts a list of stores based on the provided sort order.
        /// </summary>
        /// <param name="stores">The stores to sort.</param>
        /// <param name="sortOrder">The order to sort by (e.g., Price ascending, Price descending, Rating).</param>
        /// <returns>A sorted IQueryable of stores.</returns>
        IQueryable<Store> SortStores(IQueryable<Store> stores, string sortOrder);

        /// <summary>
        /// Paginates the list of stores.
        /// </summary>
        /// <param name="stores">The stores to paginate.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="totalPages">An output parameter returning the total number of pages.</param>
        /// <returns>A list of stores for the specified page.</returns>
        List<Store> PaginateStores(IQueryable<Store> stores, int page, int pageSize, out int totalPages);
        Task<Store> GetProductByIdAsync(int id);

    }


}
