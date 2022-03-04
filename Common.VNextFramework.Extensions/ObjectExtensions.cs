using Newtonsoft.Json;
using System;

namespace Common.VNextFramework.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToBase64(this object value)
        {
            var jsonString = JsonConvert.SerializeObject(value);
            byte[] bt = System.Text.Encoding.Default.GetBytes(jsonString);
            return Convert.ToBase64String(bt);
        }
    }
}
