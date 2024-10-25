using Microsoft.EntityFrameworkCore;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
namespace ConvicartWebApp.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ConvicartWarehouseContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ConvicartWarehouseContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Get all entities
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Get entity by ID
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Add new entity
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Update existing entity
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        // Delete entity by ID
        public async Task DeleteAsync(int id)
        {
            T entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        // Save changes to the database
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
