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
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;

namespace Quarrel.Services.Voice
{
    public sealed class VoiceService : IVoiceService
    {
        #region Public Properties

        public IAudioInService AudioInService { get; } = new AudioInService();

        public IAudioOutService AudioOutService { get; } = new AudioOutService();

        #endregion

        #region Constructor

        public VoiceService()
        {
            Messenger.Default.Register<GatewayVoiceServerUpdateMessage>(this, m => 
            {
                ConnectToVoiceChannel(m.VoiceServer, Messenger.Default.Request<CurrentUserVoiceStateRequestMessage, VoiceState>(new CurrentUserVoiceStateRequestMessage()));
            });
        }

        #endregion

        #region Variables

        private VoiceConnection _VoiceConnection;

        #endregion

        #region Methods

        public async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            AudioOutService.CreateGraph();
            _VoiceConnection = new VoiceConnection(data, state);
            _VoiceConnection.VoiceDataRecieved += VoiceDataRecieved;
            await _VoiceConnection.ConnectAsync();

            AudioInService.InputRecieved += InputRecieved;
            AudioInService.SpeakingChanged += SpeakingChanged;
            AudioInService.CreateGraph();
        }

        #endregion

        #region Helper Methods

        private void InputRecieved(object sender, float[] e)
        {
            _VoiceConnection.SendVoiceData(e);
        }

        private void VoiceDataRecieved(object sender, VoiceConnectionEventArgs<VoiceData> e)
        {
            AudioOutService.AddFrame(e.EventData.data, e.EventData.samples);
        }

        private void SpeakingChanged(object sender, int e)
        {
            _VoiceConnection.SendSpeaking(e);
        }

        #endregion
    }
}