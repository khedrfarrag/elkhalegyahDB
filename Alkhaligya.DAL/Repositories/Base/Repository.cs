using Alkhaligya.DAL.Data.DbHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Alkhaligya.DAL.Repositories.Base
{
    public class Repository<T, U> : IRepository<T, U> where T : class
    {
        protected readonly AlkhligyaContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AlkhligyaContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Get all entities as IQueryable (remains synchronous for flexible querying)
        public virtual IQueryable<T> GetAll(bool tracking = true)
        {
            var query = _context.Set<T>().AsQueryable();
            return tracking ? query : query.AsNoTracking();
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }
        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        // Get a single entity by its ID asynchronously
        public async Task<T> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        // Add a new entity asynchronously
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Update an existing entity asynchronously
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Delete an entity by ID asynchronously
        public async Task DeleteByIdAsync(U id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }
    }
}
