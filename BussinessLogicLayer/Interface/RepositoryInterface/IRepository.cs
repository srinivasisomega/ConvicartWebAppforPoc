namespace ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface
{
    public interface IRepository<T>
    {
        Task<T?> GetByIdAsync(int id);
        IQueryable<T> GetAll();
        Task AddAsync(T entity);
        void Update(T entity);
        Task SaveChangesAsync();
    }
}
