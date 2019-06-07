using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Voice.DownstreamEvents
{
    public class UnencryptedAudioHeader
    {
        public UnencryptedAudioHeader(byte[] input)
        {
            SSRC = Convert.ToUInt32(new byte[] { input[11], input[10], input[9], input[8] });
            Timestamp = Convert.ToUInt32(new byte[] { input[7], input[6], input[5], input[4] });
            Sequence = Convert.ToUInt16(new byte[] { input[3], input[2] });
            Version = input[1];
            Type = input[0];
        }

        public UnencryptedAudioHeader(byte type, byte version, ushort sequence, uint timestamp, uint ssrc)
        {
            Type = type;
            Version = version;
            Sequence = sequence;
            Timestamp = timestamp;
            SSRC = ssrc;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[12];
            bytes[0] = Type;
            bytes[1] = Version;
            byte[] seq = BitConverter.GetBytes(Sequence);
            bytes[2] = seq[0];
            bytes[3] = seq[1];
            bytes[4] = seq[2];
            return bytes;
        }

        public byte Type { get; set; }
        public byte Version { get; set; }
        public ushort Sequence { get; set; }
        public uint Timestamp { get; set; }
        public uint SSRC { get; set; }
    }
}
