using System;
using System.IO;

namespace Common.VNextFramework.Extensions
{
    public static class StringExtensions
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static void AppendTo(this string value, string filePath)
        {
            var currentFileName = Path.IsPathRooted(filePath) ? Path.Combine(AppContext.BaseDirectory, filePath) : filePath;
            if (File.Exists(currentFileName))
            {
                using var fs = new FileStream(currentFileName, FileMode.Append, FileAccess.Write);
                using StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine(value);
            }
        }
    }
}
