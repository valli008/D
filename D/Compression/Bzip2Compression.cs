using SharpCompress.Compressors.BZip2;
using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D;

namespace D.Compression
{
    public class Bzip2Compression : ICompressionAlgorithm
    {
        public string Name => "Bzip2";

        public byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                // Створюємо потік стиснення BZip2
                using (var bzip2 = new BZip2Stream(ms, SharpCompress.Compressors.CompressionMode.Compress, false))
                {
                    // Записуємо дані в потік BZip2
                    bzip2.Write(data, 0, data.Length);
                }
                // Після закриття BZip2Stream (через using), всі стиснені дані потрапляють у ms
                return ms.ToArray();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var bzip2 = new BZip2Stream(input, SharpCompress.Compressors.CompressionMode.Decompress, false))
            using (var output = new MemoryStream())
            {
                bzip2.CopyTo(output);
                return output.ToArray();
            }
        }
    }
}