using System;

namespace Common.VNextFramework.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal DecimalFloor(this decimal value, int decimals)
        {
            long multiple = Convert.ToInt64(Math.Pow(10, decimals));
            return decimal.Floor(value * multiple) / multiple;
        }
    }
}
