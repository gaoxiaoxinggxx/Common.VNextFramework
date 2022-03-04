using System.Collections.Generic;
using System.Linq;

namespace Common.VNextFramework.Extensions
{
    public static class ListExtensions
    {
        public static bool EqualsWithoutSequence<T>(this List<T> source, List<T> target)
        {
            return source.All(target.Contains) && source.Count == target.Count;
        }
    }
}
