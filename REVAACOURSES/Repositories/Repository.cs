using REVAACOURSES.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace REVAACOURSES.Repositories
{
    public class Repository<T>  : IRepository<T> where T : class
    {
        protected ApplicationDbContext _context;
        protected DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<EntityEntry<T>> AddAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T,bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked= true )
        {
            var entities = _dbSet.AsQueryable();
            if(filter !=null)
            {
                entities = entities.Where(filter);
            }
            if (includes != null)
            {
                foreach (var include in includes)
                    entities = entities.Include(include);
            }
            if(!tracked)
            {
                entities = entities.AsNoTracking();
            }
            return await entities.ToListAsync();
        }

        public async Task<T?> GetOneAsync(
           Expression<Func<T, bool>>? filter = null,
           Expression<Func<T, object>>[]? includes = null,
           bool tracked = true

           )
        {
            var entities = _dbSet.AsQueryable();
            if (filter != null)
            {
                entities = entities.Where(filter);
            }
            if (includes != null)
            {
                foreach (var include in includes)
                    entities = entities.Include(include);
            }
            if (!tracked)
            {
                entities = entities.AsNoTracking();
            }
            return await entities.FirstOrDefaultAsync();
        }


        public void UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public void DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
