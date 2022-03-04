using System;

namespace Common.VNextFramework.Extensions
{
    public static class GuidExtensions
    {
        public static string GetSalt(this Guid value, int length)
        {
            return value.ToString("N").Substring(0, length);
        }
    }
}
