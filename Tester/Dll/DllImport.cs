using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    public unsafe class DllImport
    {
        private static class SafeNativeMethods
        {
            [DllImport("libsodium", EntryPoint = "crypto_secretbox_easy", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SecretBoxEasy(byte* output, byte[] input, long inputLength, byte[] nonce, byte[] secret);
            [DllImport("libsodium", EntryPoint = "crypto_secretbox_open_easy", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SecretBoxOpenEasy(byte[] output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        }

        public static int Encrypt(byte[] input, long inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            fixed (byte* outPtr = output)
                return SafeNativeMethods.SecretBoxEasy(outPtr + outputOffset, input, inputLength, nonce, secret);
        }

        public static int Decrypt(byte[] input, int inputOffset, long inputLength, byte[] output, byte[] nonce, byte[] secret)
        {
            fixed (byte* inPtr = input)
                return SafeNativeMethods.SecretBoxOpenEasy(output, inPtr + inputLength, inputLength, nonce, secret);
        }
    }
}
