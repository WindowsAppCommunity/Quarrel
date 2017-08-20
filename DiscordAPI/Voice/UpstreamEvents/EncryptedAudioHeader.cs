using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice.DownstreamEvents
{
    public struct EncryptedAudioHeader
    {
        public EncryptedAudioHeader(byte[] packet)
        {
            Type = packet[0]; //0x80
            Version = packet[1]; //0x78
            Sequence = new byte[2];
            Sequence[1] = packet[2];
            Sequence[0] = packet[3];
            Timestamp = new byte[4];
            Timestamp[3] = packet[4];
            Timestamp[2] = packet[5];
            Timestamp[1] = packet[6];
            Timestamp[0] = packet[7];
            SSRC = new byte[4];
            SSRC[3] = packet[8];
            SSRC[2] = packet[9];
            SSRC[1] = packet[10];
            SSRC[0] = packet[11];
        }
        public byte Type { get; set; }
        public byte Version { get; set; }
        public byte[] Sequence { get; set; }
        public byte[] Timestamp { get; set; }
        public byte[] SSRC { get; set; }
    }
}
