using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace Common.VNextFramework.Tools
{
    public static class PasswordEncryptTool
    {
        public static string Encrypt(string source, string salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(source, Encoding.Default.GetBytes(salt), KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
