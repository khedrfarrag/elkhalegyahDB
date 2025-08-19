using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace  Alkhaligya.DAL.Repositories.Base
{
    public interface IRepository<T, U> where T : class
    {
        IQueryable<T> GetAll(bool tracking = true);

        Task<T> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>> include = null);

        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteByIdAsync(U id);


         Task AddRangeAsync(IEnumerable<T> entities);

        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAll(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    }
}
