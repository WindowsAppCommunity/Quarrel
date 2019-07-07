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
using System.Collections.Generic;
using Quarrel.Models.Bindables;

namespace Quarrel.Services.Voice
{
    public sealed class VoiceService : IVoiceService
    {
        #region Public Properties

        public IAudioInService AudioInService { get; } = new AudioInService();

        public IAudioOutService AudioOutService { get; } = new AudioOutService();

        #endregion

        #region Variables

        private VoiceConnection _VoiceConnection;

        // TODO: Move to UI
        private Dictionary<string, VoiceState> VoiceStates = new Dictionary<string, VoiceState>();

        #endregion

        #region Constructor

        public VoiceService()
        {
            Messenger.Default.Register<GatewayVoiceStateUpdateMessage>(this, m =>
            {
                if (VoiceStates.ContainsKey(m.VoiceState.UserId))
                {
                    VoiceStates[m.VoiceState.UserId] = m.VoiceState;
                }
                else
                {
                    VoiceStates.Add(m.VoiceState.UserId, m.VoiceState);
                }
            });

            Messenger.Default.Register<GatewayVoiceServerUpdateMessage>(this, m => 
            {
                ConnectToVoiceChannel(m.VoiceServer, VoiceStates[ServicesManager.Discord.CurrentUser.Id]);
            });
        }

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