using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface
{
    public interface IStoreRepository
    {
        IQueryable<Store> GetAllStores();
        Task<Store> GetStoreByIdAsync(int id);
    }
}
