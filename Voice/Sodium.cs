using System;
using System.Runtime.InteropServices;

namespace Discord_UWP.Voice
{
    public unsafe static class Cypher
    {

        private static int Encrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret)
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                    return EncryptARM(output, input, inputLength, nonce, secret);
                case Architecture.Arm64:
                    return EncryptARM(output, input, inputLength, nonce, secret);
                case Architecture.X64:
                    return Encrypt64(output, input, inputLength, nonce, secret);
                case Architecture.X86:
                    return Encrypt32(output, input, inputLength, nonce, secret);
            }
            //return Encrypt32(output, input, inputLength, nonce, secret);
            return -1;
        }

        private static int Decrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret)
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                    return DecryptARM(output, input, inputLength, nonce, secret);
                case Architecture.Arm64:
                    return DecryptARM(output, input, inputLength, nonce, secret);
                case Architecture.X64:
                    return Decrypt64(output, input, inputLength, nonce, secret);
                case Architecture.X86:
                    return Decrypt32(output, input, inputLength, nonce, secret);
            }
            //return Decrypt32(output, input, inputLength, nonce, secret);
            return -1;
        }

//#if X32
        //[DllImport("SodiumCWin32.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int Encrypt32(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        //[DllImport("SodiumCWin32.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int Decrypt32(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
//#endif

//#if X64
        //[DllImport("SodiumCx64.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int Encrypt64(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        //[DllImport("SodiumCx64.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int Decrypt64(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
//#endif

//#if ARM
        //[DllImport("SodiumCARM.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int EncryptARM(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        //[DllImport("SodiumCARM.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int DecryptARM(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
//#endif

        public static int encrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            #if false || true || false
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            {
                //int error = Encrypt(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
                //if (error != 0)
                //    throw new Exception($"Sodium Error: {error}");
                return inputLength + 16;
            }
            #endif
        }

        public static int decrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            {
                //int error = Decrypt(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
                //if (error != 0)
                //    throw new Exception($"Sodium Error: {error}");
                return inputLength - 16;
            }
        }
    }
}
