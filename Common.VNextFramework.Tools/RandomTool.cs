using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.VNextFramework.Tools
{
    public enum RandomStringType
    {
        Num,
        UpperAlpha,
        LowerAlpha
    }

    public class RandomTool
    {
        private static readonly string NumSource = "0123456789";
        private static readonly string UpperAlphaSource = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string LowerAlphaSource = "abcdefghijklmnopqrstuvwxyz";

        public static string Generate(RandomStringType composition, int length)
        {
            return Generate(new List<RandomStringType> { composition }, length);
        }

        public static string Generate(List<RandomStringType> compositionList, int length)
        {
            var source = "";
            var result = "";
            compositionList.ForEach(composition =>
            {
                source += (composition switch
                {
                    RandomStringType.Num => NumSource,
                    RandomStringType.UpperAlpha => UpperAlphaSource,
                    RandomStringType.LowerAlpha => LowerAlphaSource,
                    _ => ""
                });
            });

            var random = new Random();
            for (int i = 1; i <= length; i++)
            {
                var index = random.Next(0, source.Length - 1);
                result += source[index];
            }
            return result;
        }

        public static List<string> Generate(List<RandomStringType> compositionList, int length, int count)
        {
            var result = new List<string>();
            while (result.Count < count)
            {
                for (int i = 0; i < count - result.Count; i++)
                {
                    result.Add(Generate(compositionList, length));
                }
                result = result.Distinct().ToList();
            }
            return result;
        }

        public static List<string> Generate(RandomStringType composition, int length, int count)
        {
            return Generate(new List<RandomStringType> { composition }, length, count);
        }
    }
}
