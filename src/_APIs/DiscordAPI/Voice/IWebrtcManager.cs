using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Voice
{
    public interface IWebrtcManager
    {
        void Create();
        void Destroy();

        Task ConnectAsync(string readyIp, string toString, uint lastReadySsrc);

        void SetKey(byte[] key);

        void SetSpeaking(uint ssrc, int speaking);

        event EventHandler<Tuple<string, ushort>> IpAndPortObtained;
        event EventHandler<IList<float>> AudioInData;
        event EventHandler<IList<float>> AudioOutData;
        event EventHandler<bool> Speaking;
    }
}
