using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RuntimeComponent
{
    public unsafe static class Cypher
    {

#if !X86 && !X64 && !ARM //Should never happen
        private static int Encrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret)
        {
            return -1;
        }
        private static int Decrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret)
        {
            return -1;
        }
#endif

        [DllImport("SodiumC.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encrypt(byte* output, byte* input, long inputLength, byte* nonce, byte* secret);
        [DllImport("SodiumC.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Decrypt(byte* output, byte* input, long inputLength, byte* nonce, byte* secret);

        public static int encrypt([ReadOnlyArray()]byte[] input, int inputOffset, int inputLength, [WriteOnlyArray()]byte[] output, int outputOffset, [ReadOnlyArray()]byte[] nonce, [ReadOnlyArray()]byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            fixed (byte* noncePtr = nonce)
            fixed (byte* secretPtr = secret)
            {
                int error = Encrypt(outPtr + outputOffset, inPtr + inputOffset, inputLength, noncePtr, secretPtr);
                if (error != 0)
                    throw new Exception($"Sodium Error: {error}");
                return inputLength + 16;
            }
        }

        public static int decrypt([ReadOnlyArray()]byte[] input, int inputOffset, int inputLength, [WriteOnlyArray()]byte[] output, int outputOffset, [ReadOnlyArray()]byte[] nonce, [ReadOnlyArray()]byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            fixed (byte* noncePtr = nonce)
            fixed (byte* secretPtr = secret)
            {
                int error = Decrypt(outPtr + outputOffset, inPtr + inputOffset, inputLength, noncePtr, secretPtr);
                if (error != 0)
                    throw new Exception($"Sodium Error: {error}");
                return inputLength - 16;
            }
        }
    }
}
