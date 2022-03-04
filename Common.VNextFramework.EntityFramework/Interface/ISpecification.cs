using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Common.VNextFramework.EntityFramework
{
    public interface IBaseSpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        List<OrderByList<T>> OrderByList { get; }
        Expression<Func<T, object>> GroupBy { get; }

    }

    public interface ISpecification<T> : IBaseSpecification<T>
    {

    }

    public interface IPagedSpecification<T> : IBaseSpecification<T>
    {
        int PageSize { get; }

        int PageIndex { get; }
    }
}
