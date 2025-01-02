using InventoryMan.Core.Interfaces;
using InventoryMan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace InventoryMan.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly InventoryDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(InventoryDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _dbSet;
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<T>> GetPagedAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            int pageNumber = 1,
                int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var pagedQuery = query.Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize);
            
            //var items = await pagedQuery.ToListAsync(cancellationToken);
            //var sql = query.ToQueryString();
            //Console.WriteLine(sql);

            var ret = new PagedResult<T>();
            ret.CurrentPage = pageNumber;
            ret.PageSize = pageSize;
            ret.RowCount = totalCount;
            ret.Queryable = pagedQuery;


            return ret;
        }
    }
}
