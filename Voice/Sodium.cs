using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice
{
    public unsafe static class SecretBox
    {

        private static int SecretBoxEasy(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret)
        {
            switch (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                    return SecretBoxEasyArm(output, input, inputLength, nonce, secret);
                case Architecture.Arm64:
                    return SecretBoxEasyArm(output, input, inputLength, nonce, secret);
                case Architecture.X64:
                    return SecretBoxEasy64(output, input, inputLength, nonce, secret);
                case Architecture.X86:
                    return SecretBoxEasy32(output, input, inputLength, nonce, secret);
            }
            return SecretBoxEasy32(output, input, inputLength, nonce, secret);
        }

        private static int SecretBoxOpenEasy(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret)
        {
            switch (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                    return SecretBoxOpenEasyArm(output, input, inputLength, nonce, secret);
                case Architecture.Arm64:
                    return SecretBoxOpenEasyArm(output, input, inputLength, nonce, secret);
                case Architecture.X64:
                    return SecretBoxOpenEasy64(output, input, inputLength, nonce, secret);
                case Architecture.X86:
                    return SecretBoxOpenEasy32(output, input, inputLength, nonce, secret);
            }
            return SecretBoxEasy32(output, input, inputLength, nonce, secret);
        }

        #region 32
        [DllImport("SodiumC", CallingConvention = CallingConvention.StdCall)]
        private static extern int SecretBoxEasy32(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("SodiumC", CallingConvention = CallingConvention.StdCall)]
        private static extern int SecretBoxOpenEasy32(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        #endregion

        #region 64
        [DllImport("SodiumC", CallingConvention = CallingConvention.StdCall)]
        private static extern int SecretBoxEasy64(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("SodiumC", CallingConvention = CallingConvention.StdCall)]
        private static extern int SecretBoxOpenEasy64(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        #endregion

        #region Arm
        [DllImport("SodiumC", CallingConvention = CallingConvention.StdCall)]
        private static extern int SecretBoxEasyArm(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("SodiumC", CallingConvention = CallingConvention.StdCall)]
        private static extern int SecretBoxOpenEasyArm(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        #endregion


        public static int Encrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            {
                int error = SecretBoxEasy(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
                if (error != 0)
                    throw new Exception($"Sodium Error: {error}");
                return inputLength + 16;
            }
        }

        public static int Decrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            {
                int error = SecretBoxOpenEasy(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
                if (error != 0)
                    throw new Exception($"Sodium Error: {error}");
                return inputLength - 16;
            }
        }
    }
}
