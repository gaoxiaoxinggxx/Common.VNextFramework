using Common.VNextFramework.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Common.VNextFramework.EntityFramework.Extensions
{
    public static class QueryableExtensions
    {
        public static PageResultModel<T> ToDataSourceResult<T>(this IQueryable<T> queryable, PageQueryModel query)
        {
            var result = new PageResultModel<T>();
            if (query.Filters != null && query.Filters.Any())
            {
                var filterStr = string.Join(",", query.Filters?.Select((filter, index) => $"{filter.Field}==@{index}"));
                var args = query.Filters?.Select(filter => filter.Value);
            }
            if (query.Sorts != null && query.Sorts.Any())
            {
                var sortStr = string.Join(",", query.Sorts?.Select(sort => $"{sort.Field} " + (sort.Desc ? "descending " : " ")));
            }
            result.Total = queryable.Count();
            if (query.PageIndex > 1)
            {
                queryable = queryable.Skip((query.PageIndex - 1) * query.PageSize);
            }
            if (query.PageSize > 0)
            {
                queryable = queryable.Take(query.PageSize);
            }
            result.Data = queryable.ToList();
            return result;
        }

        public static async Task<PageResultModel<T>> ToDataSourceResultAsync<T>(this IQueryable<T> queryable, PageQueryModel query)
        {
            var result = new PageResultModel<T>();
            if (query.Filters != null && query.Filters.Any())
            {
                var filterStr = string.Join(",", query.Filters?.Select((filter, index) => $"{filter.Field}==@{index}"));
                var args = query.Filters?.Select(filter => filter.Value);
            }
            if (query.Sorts != null && query.Sorts.Any())
            {
                var sortStr = string.Join(",", query.Sorts?.Select(sort => $"{sort.Field} " + (sort.Desc ? "descending " : " ")));
            }
            result.Total = await queryable.CountAsync();
            if (query.PageIndex > 1)
            {
                queryable = queryable.Skip((query.PageIndex - 1) * query.PageSize);
            }
            if (query.PageSize > 0)
            {
                queryable = queryable.Take(query.PageSize);
            }
            result.Data = await queryable.ToListAsync();
            return result;
        }
    }
}
