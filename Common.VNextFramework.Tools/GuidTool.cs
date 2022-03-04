using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.VNextFramework.Tools
{
    public static class GuidTool
    {
        private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();
        public static Guid GenerateSequentialGuid()
        {
            var randomBytes = new byte[10];
            RandomNumberGenerator.GetBytes(randomBytes); 

            long timestamp = DateTime.UtcNow.Ticks / 10000L;
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            byte[] guidBytes = new byte[16];

            Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
            Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
            return new Guid(guidBytes);
        }

    }
}
