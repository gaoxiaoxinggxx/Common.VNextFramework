using Common.VNextFramework.Extensions;
using Common.VNextFramework.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.VNextFramework.EntityFramework
{
    public class EFRepository<T> : IEFAsyncRepository<T> where T : class
    {
        public Expression<Func<T, bool>> _fixedExpression = x => true;
        public readonly DbContext _DbContext;

        public EFRepository(DbContext context)
        {
            _DbContext = context;
        }

        #region Async

        public async Task<T> GetByIdAsync(object id)
        {
            return await _DbContext.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _DbContext.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _DbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            if (typeof(BaseAuditEntity).IsAssignableFrom(typeof(T)))
            {
                predicate = predicate.And(_fixedExpression);
            }
            return await _DbContext.Set<T>().Where(predicate).SingleOrDefaultAsync();
        }

        public async Task<T> GetSingleAsync(IBaseSpecification<T> spec)
        {
            return await ApplySpecification(spec).SingleOrDefaultAsync();
        }

        public async Task<T> GetLastAsync(IBaseSpecification<T> spec)
        {
            return await ApplySpecification(spec).LastOrDefaultAsync();
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate)
        {
            if (typeof(BaseAuditEntity).IsAssignableFrom(typeof(T)))
            {
                predicate = predicate.And(_fixedExpression);
            }
            return await _DbContext.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<T> GetFirstAsync(IBaseSpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            if (typeof(BaseAuditEntity).IsAssignableFrom(typeof(T)))
            {
                predicate = predicate.And(_fixedExpression);
            }
            return await _DbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<List<T>> GetListAsync(IBaseSpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<PageResultModel<T>> GetPagedListAsync(IPagedSpecification<T> spec)
        {
            var count = await GetCountAsync(spec);
            var list = await GetListAsync(spec);
            return new PageResultModel<T>(spec.PageIndex, spec.PageSize, count, list);
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            if (typeof(BaseAuditEntity).IsAssignableFrom(typeof(T)))
            {
                predicate = predicate.And(_fixedExpression);
            }
            return await _DbContext.Set<T>().Where(predicate).CountAsync();
        }

        public async Task<int> GetCountAsync(IBaseSpecification<T> spec)
        {
            return await ApplySpecification(spec, true).CountAsync();
        }


        public async Task AddAsync(T entity)
        {
            await _DbContext.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync(List<T> entities)
        {
            await _DbContext.Set<T>().AddRangeAsync(entities);
        }

        public async Task RemoveAsync(T entity)
        {
            _DbContext.Set<T>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task RemoveRangeAsync(List<T> entities)
        {
            _DbContext.Set<T>().RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task ModifyAsync(T entity)

        {
            _DbContext.Set<T>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task ModifyRangeAsync(List<T> entities)
        {
            _DbContext.Set<T>().UpdateRange(entities);
            await Task.CompletedTask;
        }

        public async Task<bool> SaveChangeAsync()
        {
            return (await _DbContext.SaveChangesAsync()) > 0;
        }
        #endregion

        private IQueryable<T> ApplySpecification(IBaseSpecification<T> specification, bool ifGetCount = false)
        {

            var query = _DbContext.Set<T>().AsQueryable();

            query = query.Where(_fixedExpression);

            // modify the IQueryable using the specification's criteria expression
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Includes all expression-based includes
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering if expressions are set
            if (specification.OrderByList.Any())
            {
                IOrderedQueryable<T> orderQuery = null;
                var isFistOrder = true;
                foreach (var item in specification.OrderByList)
                {
                    if (isFistOrder)
                    {
                        orderQuery = item.IsDescending ? query.OrderByDescending(item.OrderBy) : query.OrderBy(item.OrderBy);
                        isFistOrder = false;
                    }
                    else
                    {
                        orderQuery = item.IsDescending ? orderQuery.ThenByDescending(item.OrderBy) : orderQuery.ThenBy(item.OrderBy);
                    }
                }
                query = orderQuery;
            }

            if (specification.GroupBy != null)
            {
                query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
            }

            // Apply paging if enabled
            if (!ifGetCount && specification is IPagedSpecification<T> pagedSpecification)
            {
                query = query.Skip((pagedSpecification.PageIndex - 1) * pagedSpecification.PageSize).Take(pagedSpecification.PageSize);
            }
            return query;
        }

    }
}
