using Common.VNextFramework.Extensions;
using Common.VNextFramework.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Common.VNextFramework.EntityFramework
{

    public class BaseSpecification<T> : IBaseSpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; private set; } = x => true;
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public List<OrderByList<T>> OrderByList { get; private set; } = new List<OrderByList<T>>();
        public Expression<Func<T, object>> GroupBy { get; private set; }

        public void AddPredicate(Expression<Func<T, bool>> predicateExpression)
        {
            Criteria = Criteria.And(predicateExpression);
        }
        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderByList.Add(new OrderByList<T>() { OrderBy = orderByExpression, IsDescending = false });
        }
        public void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByList.Add(new OrderByList<T>() { OrderBy = orderByDescendingExpression, IsDescending = true });
        }

        //Not used anywhere at the moment, but someone requested an example of setting this up.
        public void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }

    }

    public class Specification<T> : BaseSpecification<T>, ISpecification<T>
    {
        private Specification()
        {

        }
        public static Specification<T> GetSpecification()
        {
            return new Specification<T>();
        }
    }

    public class PagedSpecification<T> : BaseSpecification<T>, IPagedSpecification<T>
    {
        private PagedSpecification()
        {

        }
        private PagedSpecification(PageQueryModel query)
        {
            PageIndex = query.PageIndex;
            PageSize = query.PageSize;
        }
        public static PagedSpecification<T> GetSpecification(PageQueryModel query)
        {
            return new PagedSpecification<T>(query);
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

    }


    public class OrderByList<T>
    {
        public Expression<Func<T, object>> OrderBy { get; set; }

        public bool IsDescending { get; set; }
    }
}
