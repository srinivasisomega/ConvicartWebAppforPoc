using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.DataAccessLayer.Data;

namespace ConvicartWebApp.DataAccessLayer.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ConvicartWarehouseContext _context;

        public Repository(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
