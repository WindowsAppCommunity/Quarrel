using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Voice
{
    public interface IWebrtcManager
    {
        event EventHandler<Tuple<string, ushort>> IpAndPortObtained;
        event EventHandler<float[]> AudioInData;
        event EventHandler<float[]> AudioOutData;

        Task ConnectAsync(string readyIp, string toString, uint lastReadySsrc);

        void SetKey(byte[] key);

        void SetSpeaking(uint ssrc, int speaking);
    }
}
