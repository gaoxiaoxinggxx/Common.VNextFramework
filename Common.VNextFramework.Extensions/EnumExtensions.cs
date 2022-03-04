using System;
using System.ComponentModel;
using System.Reflection;

namespace Common.VNextFramework.Extensions
{
    /// <summary>
    /// EnumExtend
    /// </summary>
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memberInfos = type.GetMember(en.ToString());
            if (memberInfos.Length > 0)
            {
                if (memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attrs && attrs.Length > 0)
                {
                    return attrs[0].Description;
                }
            }
            return en.ToString();
        }
    }
}
