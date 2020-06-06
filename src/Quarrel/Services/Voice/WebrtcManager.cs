using DiscordAPI.Voice;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;

namespace Quarrel.Services.Voice
{
    /// <summary>
    /// Wraps <see cref="Webrtc.WebrtcManager"/> so that the DiscordAPI project can access it
    /// </summary>
    public class WebrtcManager : IWebrtcManager
    {
        private Webrtc.WebrtcManager manager;
        private VoipPhoneCall voipCall;
        private ISettingsService _settingsService;


        public event EventHandler<Tuple<string, ushort>> IpAndPortObtained;
        public event EventHandler<IList<float>> AudioInData;
        public event EventHandler<IList<float>> AudioOutData;
        public event EventHandler<bool> Speaking;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebrtcManager"/> class.
        /// </summary>
        public WebrtcManager(string inputDeviceId, string outputDeviceId)
        {
            manager = new Webrtc.WebrtcManager(inputDeviceId, outputDeviceId);
            manager.IpAndPortObtained += (ip, port) => IpAndPortObtained.Invoke(this, new Tuple<string, ushort>(ip, port));
            manager.AudioInData += (sender, data) => 
                AudioInData?.Invoke(sender, data);
            manager.AudioOutData += (sender, data) =>
                AudioOutData?.Invoke(sender, data);
            manager.Speaking += (sender, data) =>
                Speaking?.Invoke(sender, data);
        }

        private ISettingsService SettingsService => _settingsService ?? (_settingsService = SimpleIoc.Default.GetInstance<ISettingsService>());

        public void Create()
        {
            manager.Create();

            // TODO: More info for Mobile
            VoipCallCoordinator vcc = VoipCallCoordinator.GetDefault();
            voipCall = vcc.RequestNewOutgoingCall(string.Empty, string.Empty, "Quarrel", VoipPhoneCallMedia.Audio);
            voipCall.NotifyCallActive();
        }

        public void Destroy()
        {
            manager.Destroy();
            //voipCall.NotifyCallEnded();
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

        public void SetPlaybackDevice(string deviceId)
        {
            manager.SetPlaybackDevice(deviceId);
        }
        public void SetRecordingDevice(string deviceId)
        {
            manager.SetRecordingDevice(deviceId);
        }
    }
}
