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
    public class DeflateCompression : ICompressionAlgorithm
    {
        public string Name => "Deflate";

        public byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            using (var deflate = new DeflateStream(ms, CompressionLevel.Optimal))
            {
                deflate.Write(data, 0, data.Length);
                deflate.Close();
                return ms.ToArray();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                deflate.CopyTo(output);
                return output.ToArray();
            }
        }
    }

}
