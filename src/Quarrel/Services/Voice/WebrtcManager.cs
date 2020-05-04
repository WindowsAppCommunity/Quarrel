using DiscordAPI.Voice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Services.Voice
{
    /// <summary>
    /// Wraps <see cref="Webrtc.WebrtcManager"/> so that the DiscordAPI project can access it
    /// </summary>
    public class WebrtcManager : IWebrtcManager
    {
        private Webrtc.WebrtcManager manager;

        public event EventHandler<Tuple<string, ushort>> IpAndPortObtained;
        public event EventHandler<float[]> AudioInData;
        public event EventHandler<float[]> AudioOutData;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebrtcManager"/> class.
        /// </summary>
        public WebrtcManager()
        {
            manager = new Webrtc.WebrtcManager();
            manager.IpAndPortObtained += (ip, port) => IpAndPortObtained.Invoke(this, new Tuple<string, ushort>(ip, port));
          /*  manager.AudioInData += (sender, data) => 
                AudioInData.Invoke(this, (float[])data);
            manager.AudioOutData += (sender, data) =>
                AudioOutData.Invoke(this, (float[])data);*/
        }


        public async Task ConnectAsync(string ip, string port, uint ssrc)
        {
            await manager.ConnectAsync(ip, port, ssrc);
        }

        public void SetKey(byte[] key)
        {
            manager.SetKey(key);
        }

        public void SetSpeaking(uint ssrc, int speaking)
        {
            manager.SetSpeaking(ssrc, speaking);
        }
    }
}
