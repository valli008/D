using K4os.Compression.LZ4;
using System.IO.Compression;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using D;

namespace D.Compression
{
    public class LZ4Compression : ICompressionAlgorithm
    {
        public string Name => "LZ4";

        // Метод для стиснення
        public byte[] Compress(byte[] data)
        {
            // Використовуємо високорівневий метод стиснення з K4os.Compression.LZ4
            return LZ4Pickler.Pickle(data);
        }

        // Метод для розпакування
        public byte[] Decompress(byte[] data)
        {
            // Використовуємо високорівневий метод розпакування з K4os.Compression.LZ4
            return LZ4Pickler.Unpickle(data);
        }
    }
}