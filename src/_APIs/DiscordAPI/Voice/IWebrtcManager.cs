// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAPI.Voice
{
    /// <summary>
    /// Wraps <see cref="Webrtc.WebrtcManager"/> so that the DiscordAPI project can access it.
    /// </summary>
    public interface IWebrtcManager
    {
        /// <summary>
        /// Fired when the IP and port obtained,
        /// </summary>
        event EventHandler<Tuple<string, ushort>> IpAndPortObtained;

        /// <summary>
        /// Fired when input data is recieved.
        /// </summary>
        event EventHandler<IList<float>> AudioInData;

        /// <summary>
        /// Fired when output data is recieved.
        /// </summary>
        event EventHandler<IList<float>> AudioOutData;

        /// <summary>
        /// Fired when the user's speaking status changes.
        /// </summary>
        event EventHandler<bool> Speaking;

        /// <summary>
        /// Initializes the <see cref="Webrtc.WebrtcManager"/>.
        /// </summary>
        void Create();

        /// <summary>
        /// Disposes the <see cref="Webrtc.WebrtcManager"/>.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Connects to a voice server.
        /// </summary>
        /// <param name="readyIp">The server's IP.</param>
        /// <param name="toString">The socket's port.</param>
        /// <param name="lastReadySsrc">The user's ssrc.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ConnectAsync(string readyIp, string toString, uint lastReadySsrc);

        /// <summary>
        /// Sets the key for encryption/decryption.
        /// </summary>
        /// <param name="key">The encryption key.</param>
        void SetKey(byte[] key);

        /// <summary>
        /// Sets the speaking status.
        /// </summary>
        /// <param name="ssrc">The user's ssrc.</param>
        /// <param name="speaking">Whether or not the user's speaking.</param>
        void SetSpeaking(uint ssrc, int speaking);

        /// <summary>
        /// Sets the playback device.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        void SetPlaybackDevice(string deviceId);

        /// <summary>
        /// Sets the recording device.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        void SetRecordingDevice(string deviceId);
    }
}
