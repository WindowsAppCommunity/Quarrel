using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice
{
    //TODO: Encrypt
    public unsafe static class Cypher
    {
        private static int Encrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret)
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                    return EncryptArm(output, input, inputLength, nonce, secret);
                case Architecture.Arm64:
                    return EncryptArm(output, input, inputLength, nonce, secret);
                case Architecture.X64:
                    return Encrypt64(output, input, inputLength, nonce, secret);
                case Architecture.X86:
                    return Encrypt32(output, input, inputLength, nonce, secret);
            }
            return Encrypt32(output, input, inputLength, nonce, secret);
        }

        private static int Decrypt(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret)
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                    return DecryptArm(output, input, inputLength, nonce, secret);
                case Architecture.Arm64:
                    return DecryptArm(output, input, inputLength, nonce, secret);
                case Architecture.X64:
                    return Decrypt64(output, input, inputLength, nonce, secret);
                case Architecture.X86:
                    return Decrypt32(output, input, inputLength, nonce, secret);
            }
            return Decrypt32(output, input, inputLength, nonce, secret);
        }

        #region 32
        [DllImport("SodiumCWin32.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encrypt32(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("SodiumCWin32.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Decrypt32(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        #endregion

        #region 64
        [DllImport("SodiumCx64.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encrypt64(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("SodiumCx64.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Decrypt64(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        #endregion

        #region Arm
        [DllImport("SodiumCARM.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int EncryptArm(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("SodiumCARM.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DecryptArm(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        #endregion

        public static int encrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
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

        public static int decrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
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
