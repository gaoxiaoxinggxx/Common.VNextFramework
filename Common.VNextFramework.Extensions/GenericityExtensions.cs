using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.VNextFramework.Extensions
{
    public static class GenericityExtensions
    {
        public static object ExpressionGetTarget<T>(this T h, Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Parameter:
                    return h;
                case ExpressionType.MemberAccess:
                    MemberExpression mex = (MemberExpression)expr;
                    PropertyInfo pi = mex.Member as PropertyInfo;
                    if (pi == null) throw new ArgumentException();
                    object target = ExpressionGetTarget(h, mex.Expression);
                    return pi.GetValue(target, null);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
