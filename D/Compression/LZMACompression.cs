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
    public class LZMACompression : ICompressionAlgorithm
    {
        public string Name => "LZMA";

        public byte[] Compress(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data), "Дані не можуть бути порожніми.");

            var encoder = new SevenZip.Compression.LZMA.Encoder();
            using (var ms = new MemoryStream())
            {
                encoder.WriteCoderProperties(ms);
                long fileSize = data.Length;
                for (int i = 0; i < 8; i++)
                    ms.WriteByte((byte)(fileSize >> (8 * i)));

                using (var input = new MemoryStream(data))
                {
                    encoder.Code(input, ms, data.Length, -1, null);
                }
                return ms.ToArray();
            }
        }
        public byte[] Decompress(byte[] data)
        {
            var decoder = new SevenZip.Compression.LZMA.Decoder();
            using (var input = new MemoryStream(data))
            using (var output = new MemoryStream())
            {
                byte[] properties = new byte[5];
                input.Read(properties, 0, 5);

                long outSize = 0;
                for (int i = 0; i < 8; i++)
                {
                    int v = input.ReadByte();
                    outSize |= ((long)v) << (8 * i);
                }

                decoder.SetDecoderProperties(properties);
                decoder.Code(input, output, input.Length, outSize, null);

                return output.ToArray();
            }
        }
    }
}
