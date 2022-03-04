using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.VNextFramework.Extensions
{
    /// <summary>
    /// DistinctBy
    /// </summary>
    public static class AttributeExtensions
    {

        public static T GetAttribute<T>(Type classType) where T:Attribute
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(classType);   
            var attr = attrs.Single(x => x.GetType() == typeof(T));
            return (T)attr;
        }
    }
}
