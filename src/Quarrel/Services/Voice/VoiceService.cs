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
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Messages.Voice;
using Quarrel.Models.Bindables;

namespace Quarrel.Services.Voice
{
    public sealed class VoiceService : IVoiceService
    {
        #region Public Properties

        public IAudioInService AudioInService { get; } = SimpleIoc.Default.GetInstance<IAudioInService>();

        public IAudioOutService AudioOutService { get; } = SimpleIoc.Default.GetInstance<IAudioOutService>();

        #endregion

        #region Variables

        private VoiceConnection _VoiceConnection;

        public Dictionary<string, VoiceState> VoiceStates { get; } = new Dictionary<string, VoiceState>();

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

            Messenger.Default.Register<GatewayReadyMessage>(this, m => 
            {
                foreach (var guild in m.EventData.Guilds)
                {
                    if(guild.VoiceStates != null)
                    {
                        foreach (var state in guild.VoiceStates)
                        {
                            if (VoiceStates.ContainsKey(state.UserId))
                            {
                                VoiceStates[state.UserId] = state;
                            }
                            else
                            {
                                VoiceStates.Add(state.UserId, state);
                            }
                        }
                    }
                }
            });
        }

        #endregion

        #region Methods

        public async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            AudioOutService.CreateGraph();
            _VoiceConnection = new VoiceConnection(data, state);
            _VoiceConnection.VoiceDataRecieved += VoiceDataRecieved;
            _VoiceConnection.Speak += Speak;
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
        private void Speak(object sender, VoiceConnectionEventArgs<Speak> e)
        {
            Messenger.Default.Send(new SpeakMessage(e.EventData));
        }

        private void SpeakingChanged(object sender, int e)
        {
            _VoiceConnection.SendSpeaking(e);
        }

        #endregion
    }
}