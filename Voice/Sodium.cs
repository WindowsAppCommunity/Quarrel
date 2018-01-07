using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice
{
    public unsafe static class Cypher
    {
        private static int StreamCypher(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret)
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                    return StreamCypherArm(output, input, inputLength, nonce, secret);
                case Architecture.Arm64:
                    return StreamCypherArm(output, input, inputLength, nonce, secret);
                case Architecture.X64:
                    return StreamCypher64(output, input, inputLength, nonce, secret);
                case Architecture.X86:
                    return StreamCypher32(output, input, inputLength, nonce, secret);
            }
            return StreamCypher32(output, input, inputLength, nonce, secret);
        }

        #region 32
        [DllImport("SodiumCWin32.dll", EntryPoint = "StreamCypher", CallingConvention = CallingConvention.Cdecl)]
        private static extern int StreamCypher32(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        #endregion

        #region 64
        [DllImport("SodiumCx64.dll", EntryPoint = "StreamCypher", CallingConvention = CallingConvention.Cdecl)]
        private static extern int StreamCypher64(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        #endregion

        #region Arm
        [DllImport("SodiumCARM.dll", EntryPoint = "StreamCypher", CallingConvention = CallingConvention.Cdecl)]
        private static extern int StreamCypherArm(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        #endregion

        public static int process(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            {
                int error = StreamCypher(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
                if (error != 0)
                    throw new Exception($"Sodium Error: {error}");
                return inputLength + 16;
            }
        }
    }
}
