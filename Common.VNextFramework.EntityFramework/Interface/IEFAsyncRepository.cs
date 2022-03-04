using Common.VNextFramework.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.VNextFramework.EntityFramework
{
    public interface IEFAsyncRepository<T>
    {
        Task<bool> SaveChangeAsync();

        Task<T> GetByIdAsync(object id);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetSingleAsync(IBaseSpecification<T> spec);
        Task<T> GetLastAsync(IBaseSpecification<T> spec);
        Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetFirstAsync(IBaseSpecification<T> spec);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetListAsync(IBaseSpecification<T> spec);
        Task<PageResultModel<T>> GetPagedListAsync(IPagedSpecification<T> spec);
        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> entities);

        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(List<T> entities);

        Task ModifyAsync(T entity);
        Task ModifyRangeAsync(List<T> entities);

        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);
        Task<int> GetCountAsync(IBaseSpecification<T> spec);
    }
}
