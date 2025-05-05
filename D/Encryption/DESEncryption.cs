using D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace D.Encryption
{
    public class DESEncryption : IEncryptionAlgorithm
    {
        public string Name => "DES";

        public byte[] Encrypt(byte[] data, string password)
        {
            using (var des = DES.Create())
            {
                var key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("salt"));
                des.Key = key.GetBytes(des.KeySize / 8);
                des.IV = key.GetBytes(des.BlockSize / 8);

                using (var ms = new MemoryStream())
                using (var cryptoStream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] data, string password)
        {
            using (var des = DES.Create())
            {
                var key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("salt"));
                des.Key = key.GetBytes(des.KeySize / 8);
                des.IV = key.GetBytes(des.BlockSize / 8);

                using (var ms = new MemoryStream())
                using (var cryptoStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
    }
}
