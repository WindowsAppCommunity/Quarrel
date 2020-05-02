using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordAPI.Voice
{
    public interface IWebrtcManager
    {
        event EventHandler<Tuple<string, ushort>> IpAndPortObtained;

        void ConnectAsync(string readyIp, string toString, uint lastReadySsrc);

        void SetKey(byte[] key);

        void SetSpeaking(uint ssrc, int speaking);
    }
}
