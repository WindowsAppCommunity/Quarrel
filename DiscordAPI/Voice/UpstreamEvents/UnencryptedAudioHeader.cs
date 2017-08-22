using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice.DownstreamEvents
{
    public struct UnencryptedAudioHeader
    {
        public UnencryptedAudioHeader(byte[] input)
        {
            SSRC = Convert.ToUInt32(new byte[] { input[11], input[10], input[9], input[8] });
            Timestamp = Convert.ToUInt32(new byte[] { input[7], input[6], input[5], input[4] });
            Sequence = Convert.ToUInt16(new byte[] { input[3], input[2] });
            Version = input[1];
            Type = input[0];
        }

        public byte Type { get; set; }
        public byte Version { get; set; }
        public ushort Sequence { get; set; }
        public uint Timestamp { get; set; }
        public uint SSRC { get; set; }
    }
}
