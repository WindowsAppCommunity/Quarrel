using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RuntimeComponent
{
    public unsafe static class Cypher
    {

#if X86
        [DllImport("SodiumCWin32.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("SodiumCWin32.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Decrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
#endif

#if X64
        [DllImport("SodiumCx64.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("SodiumCx64.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Decrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
#endif

#if ARM
        [DllImport("SodiumCARM.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("SodiumCARM.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Decrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
#endif

        public static int encrypt([ReadOnlyArray()]byte[] input, int inputOffset, int inputLength, [WriteOnlyArray()]byte[] output, int outputOffset, [ReadOnlyArray()]byte[] nonce, [ReadOnlyArray()]byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            {
                int error = Encrypt(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
                if (error != 0)
                    throw new Exception($"Sodium Error: {error}");
                return inputLength + 16;
            }
        }

        public static int decrypt([ReadOnlyArray()]byte[] input, int inputOffset, int inputLength, [WriteOnlyArray()]byte[] output, int outputOffset, [ReadOnlyArray()]byte[] nonce, [ReadOnlyArray()]byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            {
                int error = Decrypt(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
                if (error != 0)
                    throw new Exception($"Sodium Error: {error}");
                return inputLength - 16;
            }
        }
    }
}
