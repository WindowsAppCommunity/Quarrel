using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI
{
    public static class Utility
    {
        public static string DeflateGZip(string hexinput)
        {
            var _compressed = new MemoryStream();
            var _decompressor = new DeflateStream(_compressed, CompressionMode.Decompress);
            using (var ms = new MemoryStream(Convert.FromBase64String(hexinput)))
            {
                ms.Position = 0;
                byte[] data = new byte[ms.Length];
                ms.Read(data, 0, (int)ms.Length);
                int index = 0;
                int count = data.Length;
                using (var decompressed = new MemoryStream())
                {
                    if (data[0] == 0x78)
                    {
                        _compressed.Write(data, index + 2, count - 2);
                        _compressed.SetLength(count - 2);
                    }
                    else
                    {
                        _compressed.Write(data, index, count);
                        _compressed.SetLength(count);
                    }

                    _compressed.Position = 0;
                    _decompressor.CopyTo(decompressed);
                    _compressed.Position = 0;
                    decompressed.Position = 0;
                    using (var reader = new StreamReader(decompressed))
                        return reader.ReadToEnd();
                }
            }
        }
        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
