using D;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D.Encryption
{
    public class BlowfishEncryption : IEncryptionAlgorithm
    {
        public string Name => "Blowfish";

        public byte[] Encrypt(byte[] data, string password)
        {
            BlowfishEngine engine = new BlowfishEngine();
            byte[] key = Encoding.UTF8.GetBytes(password);
            engine.Init(true, new KeyParameter(key));

            int blockSize = engine.GetBlockSize();
            int paddedLength = ((data.Length + blockSize - 1) / blockSize) * blockSize;

            byte[] paddedData = new byte[paddedLength];
            Array.Copy(data, paddedData, data.Length);

            byte[] encrypted = new byte[paddedLength];
            for (int i = 0; i < paddedLength; i += blockSize)
            {
                engine.ProcessBlock(paddedData, i, encrypted, i);
            }
            return encrypted;
        }

        public byte[] Decrypt(byte[] data, string password)
        {
            BlowfishEngine engine = new BlowfishEngine();
            byte[] key = Encoding.UTF8.GetBytes(password);
            engine.Init(false, new KeyParameter(key));

            int blockSize = engine.GetBlockSize();
            byte[] decrypted = new byte[data.Length];

            for (int i = 0; i < data.Length; i += blockSize)
            {
                engine.ProcessBlock(data, i, decrypted, i);
            }
            return decrypted;
        }
    }
}
