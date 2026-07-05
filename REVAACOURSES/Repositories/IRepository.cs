using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace REVAACOURSES.Repositories
{
    public interface IRepository<T> where T : class
    {
          Task<EntityEntry<T>> AddAsync(T entity);

          Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true

            );

          Task<T?> GetOneAsync(
           Expression<Func<T, bool>>? filter = null,
           Expression<Func<T, object>>[]? includes = null,
           bool tracked = true

           );


         void UpdateAsync(T entity);

         void DeleteAsync(T entity);

          Task<int> CommitAsync();
      
    }
}
