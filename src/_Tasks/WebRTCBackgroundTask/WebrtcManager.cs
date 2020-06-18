// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;
using DiscordAPI.Voice;

namespace WebRTCBackgroundTask
{
    /// <summary>
    /// Wraps <see cref="Webrtc.WebrtcManager"/> so that the DiscordAPI project can access it.
    /// </summary>
    internal class WebrtcManager : IWebrtcManager
    {
        private Webrtc.WebrtcManager manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebrtcManager"/> class.
        /// </summary>
        /// <param name="inputDeviceId">The id of the input device.</param>
        /// <param name="outputDeviceId">The id of the output device.</param>
        public WebrtcManager()
        {
            manager = new Webrtc.WebrtcManager();
            manager.IpAndPortObtained += (ip, port) => IpAndPortObtained.Invoke(this, new Tuple<string, ushort>(ip, port));
            manager.AudioInData += (sender, data) => AudioInData?.Invoke(sender, data);
            manager.AudioOutData += (sender, data) => AudioOutData?.Invoke(sender, data);
            manager.Speaking += (sender, data) => Speaking?.Invoke(sender, data);
        }

        /// <inheritdoc/>
        public event EventHandler<Tuple<string, ushort>> IpAndPortObtained;

        /// <inheritdoc/>
        public event EventHandler<IList<float>> AudioInData;

        /// <inheritdoc/>
        public event EventHandler<IList<float>> AudioOutData;

        /// <inheritdoc/>
        public event EventHandler<bool> Speaking;

        /// <inheritdoc/>
        public void Create()
        {
            manager.Create();
        }

        /// <inheritdoc/>
        public void Destroy()
        {
            manager.Destroy();
        }

        /// <inheritdoc/>
        public async Task ConnectAsync(string ip, string port, uint ssrc)
        {
            await manager.ConnectAsync(ip, port, ssrc);
        }

        /// <inheritdoc/>
        public void SetKey(byte[] key)
        {
            manager.SetKey(key);
        }

        /// <inheritdoc/>
        public void SetSpeaking(uint ssrc, int speaking)
        {
            manager.SetSpeaking(ssrc, speaking);
        }

        /// <inheritdoc/>
        public void SetPlaybackDevice(string deviceId)
        {
            manager.SetPlaybackDevice(deviceId);
        }

        /// <inheritdoc/>
        public void SetRecordingDevice(string deviceId)
        {
            manager.SetRecordingDevice(deviceId);
        }
    }
}
