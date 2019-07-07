// Special thanks to Sergio Pedri for the basis of this design

using System;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.Messages.Services.Settings;
using Quarrel.Services.Settings.Enums;
using Quarrel.Services.Voice.Audio.In;
using Quarrel.Services.Voice.Audio.Out;
using DiscordAPI.Models;
using DiscordAPI.Voice;
using DiscordAPI.Voice.DownstreamEvents;

namespace Quarrel.Services.Voice
{
    public sealed class VoiceService : IVoiceService
    {
        #region Public Properties

        public IAudioInService InAudioService { get; } = new AudioInService();

        public IAudioOutService OutAudioService { get; } = new AudioOutService();

        #endregion

        #region Variables

        private VoiceConnection _VoiceConnection;

        #endregion

        #region Methods

        public async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            _VoiceConnection = new VoiceConnection(data, state);
            _VoiceConnection.VoiceDataRecieved += _VoiceConnection_VoiceDataRecieved;
            await _VoiceConnection.ConnectAsync();
            InAudioService.InputRecieved += InAudioService_InputRecieved;
        }

        private void InAudioService_InputRecieved(object sender, float[] e)
        {
            _VoiceConnection.SendVoiceData(e);
        }

        private void _VoiceConnection_VoiceDataRecieved(object sender, VoiceConnectionEventArgs<VoiceData> e)
        {
            OutAudioService.AddFrame(e.EventData.data, e.EventData.samples);
        }

        #endregion
    }
}