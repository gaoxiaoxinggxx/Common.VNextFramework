using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common.VNextFramework.Tools.Encrypts
{
    /// <summary>
    /// 内部实现
    /// key转byte[32],超出丢弃,不足补空格(ASCII 32) 
    /// iv转byte[16],超出丢弃,不足补空格(ASCII 32) 
    /// </summary>
    public class AESEncryptTool
    {
        public Aes AesInstance { get; set; }

        public AESEncryptTool(string key, string iv = null, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            AesInstance = Aes.Create();
            AesInstance.Mode = mode;
            AesInstance.Padding = padding;

            // 创建一个Aes额定长度的Key(使用空格填充),然后使用inputKey覆盖
            var aesByteKey = Encoding.ASCII.GetBytes(new string(' ', AesInstance.Key.Length));
            var inputByteKey = Encoding.UTF8.GetBytes(key);
            Buffer.BlockCopy(inputByteKey, 0, aesByteKey, 0, Math.Min(inputByteKey.Length, AesInstance.Key.Length));
            AesInstance.Key = aesByteKey;


            // 创建一个Aes额定长度的Iv(使用空格填充),然后使用inputIv覆盖
            var aesByteIv = Encoding.ASCII.GetBytes(new string(' ', AesInstance.IV.Length));
            var inputByteIv = iv == null ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(iv);
            Buffer.BlockCopy(inputByteIv, 0, aesByteIv, 0, Math.Min(inputByteIv.Length, AesInstance.IV.Length));
            AesInstance.IV = aesByteIv;
        }

        public string Encrypt(string str)
        {
            byte[] inBytes = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, AesInstance.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inBytes, 0, inBytes.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        public string Decrypt(string str)
        {
            byte[] inBytes = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, AesInstance.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inBytes, 0, inBytes.Length);
            cs.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray()); ;
        }
    }
}
