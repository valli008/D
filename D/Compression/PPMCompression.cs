using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using D;

namespace D.Compression
{
    public class PPMCompression : ICompressionAlgorithm
    {
        public string Name => "PPM";

        private const int MAX_CONTEXT_SIZE = 4; // Максимальна глибина контексту
        private const byte ESCAPE_SYMBOL = 255; // Символ "вихід"

        // Клас для роботи з вузлами в моделі PPM
        private class PpmNode
        {
            public Dictionary<byte, SymbolInfo> Symbols { get; } = new Dictionary<byte, SymbolInfo>();
            public int TotalCount { get; set; } = 0;

            public void AddSymbol(byte symbol)
            {
                if (!Symbols.TryGetValue(symbol, out SymbolInfo info))
                {
                    info = new SymbolInfo();
                    Symbols[symbol] = info;
                }

                info.Count++;
                TotalCount++;
            }

            public byte[] GetSymbolsArray()
            {
                var result = new byte[Symbols.Count];
                int index = 0;

                foreach (var symbol in Symbols.Keys)
                {
                    result[index++] = symbol;
                }

                return result;
            }
        }

        // Інформація про символ
        private class SymbolInfo
        {
            public int Count { get; set; } = 0;
        }

        public byte[] Compress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new byte[0];

            using (var output = new MemoryStream())
            using (var writer = new BinaryWriter(output))
            {
                // Запис розміру вхідних даних для перевірки при декомпресії
                writer.Write(data.Length);

                // Словник для зберігання контекстів різної довжини
                var models = new Dictionary<string, PpmNode>();

                // Буфер для поточного контексту
                var contextBuffer = new List<byte>();

                // Стиснення даних
                for (int i = 0; i < data.Length; i++)
                {
                    byte symbol = data[i];

                    // Оновлюємо всі моделі контексту
                    for (int ctxSize = 0; ctxSize <= Math.Min(MAX_CONTEXT_SIZE, contextBuffer.Count); ctxSize++)
                    {
                        string ctx = GetContextString(contextBuffer, ctxSize);

                        if (!models.TryGetValue(ctx, out PpmNode node))
                        {
                            node = new PpmNode();
                            models[ctx] = node;
                        }

                        node.AddSymbol(symbol);
                    }

                    // Кодуємо символ з використанням найдовшого контексту
                    string longestContext = GetContextString(contextBuffer, Math.Min(MAX_CONTEXT_SIZE, contextBuffer.Count));
                    EncodeSymbol(writer, models, longestContext, symbol);

                    // Оновлюємо контекстний буфер
                    contextBuffer.Add(symbol);
                    if (contextBuffer.Count > MAX_CONTEXT_SIZE)
                    {
                        contextBuffer.RemoveAt(0);
                    }
                }

                return output.ToArray();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            if (data == null || data.Length <= 4)
                return new byte[0];

            using (var input = new MemoryStream(data))
            using (var reader = new BinaryReader(input))
            using (var output = new MemoryStream())
            {
                // Читаємо розмір оригінальних даних
                int originalSize = reader.ReadInt32();

                // Словник для зберігання контекстів різної довжини
                var models = new Dictionary<string, PpmNode>();

                // Буфер для поточного контексту
                var contextBuffer = new List<byte>();

                // Декомпресія даних
                for (int i = 0; i < originalSize; i++)
                {
                    // Шукаємо символ на основі найдовшого контексту
                    string longestContext = GetContextString(contextBuffer, Math.Min(MAX_CONTEXT_SIZE, contextBuffer.Count));
                    byte symbol = DecodeSymbol(reader, models, longestContext);

                    // Записуємо символ у вихідний потік
                    output.WriteByte(symbol);

                    // Оновлюємо всі моделі контексту
                    for (int ctxSize = 0; ctxSize <= Math.Min(MAX_CONTEXT_SIZE, contextBuffer.Count); ctxSize++)
                    {
                        string ctx = GetContextString(contextBuffer, ctxSize);

                        if (!models.TryGetValue(ctx, out PpmNode node))
                        {
                            node = new PpmNode();
                            models[ctx] = node;
                        }

                        node.AddSymbol(symbol);
                    }

                    // Оновлюємо контекстний буфер
                    contextBuffer.Add(symbol);
                    if (contextBuffer.Count > MAX_CONTEXT_SIZE)
                    {
                        contextBuffer.RemoveAt(0);
                    }
                }

                return output.ToArray();
            }
        }

        // Кодування символу з використанням арифметичного кодування
        private void EncodeSymbol(BinaryWriter writer, Dictionary<string, PpmNode> models, string context, byte symbol)
        {
            // Спрощене кодування - просто записуємо символ
            // У повноцінній реалізації тут має бути арифметичне кодування
            writer.Write(symbol);
        }

        // Декодування символу
        private byte DecodeSymbol(BinaryReader reader, Dictionary<string, PpmNode> models, string context)
        {
            // Спрощене декодування - просто читаємо символ
            // У повноцінній реалізації тут має бути арифметичне декодування
            return reader.ReadByte();
        }

        // Отримання рядкового представлення контексту
        private string GetContextString(List<byte> buffer, int length)
        {
            if (length == 0)
                return string.Empty;

            int startIndex = buffer.Count - length;
            var contextBytes = new byte[length];

            for (int i = 0; i < length; i++)
            {
                contextBytes[i] = buffer[startIndex + i];
            }

            return Convert.ToBase64String(contextBytes);
        }
    }
}