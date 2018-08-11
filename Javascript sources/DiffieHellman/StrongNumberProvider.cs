using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices.WindowsRuntime;

namespace DiffieHellman
{
    internal class StrongNumberProvider
    {

        public uint NextUInt32()
        {
            return BitConverter.ToUInt32(Windows.Security.Cryptography.CryptographicBuffer.GenerateRandom(4).ToArray(), 0);
        }

        public int NextInt()
        {
            return BitConverter.ToInt32(Windows.Security.Cryptography.CryptographicBuffer.GenerateRandom(4).ToArray(), 0);
        }

        public Single NextSingle()
        {
            float numerator = NextUInt32();
            float denominator = uint.MaxValue;
            return numerator / denominator;
        }
    }
}
