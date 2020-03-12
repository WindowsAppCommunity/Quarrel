// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Voice;
using DiscordAPI.Voice.DownstreamEvents;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Voice;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Voice.Audio.In;
using Quarrel.ViewModels.Services.Voice.Audio.Out;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Voice
{
    /// <summary>
    /// Manages all voice state data.
    /// </summary>
    public sealed class VoiceService : IVoiceService
    {
        private VoiceConnection _voiceConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceService"/> class.
        /// </summary>
        public VoiceService()
        {
            Messenger.Default.Register<GatewayVoiceStateUpdateMessage>(this, m =>
            {
                if (VoiceStates.ContainsKey(m.VoiceState.UserId))
                {
                    var channel = SimpleIoc.Default.GetInstance<IChannelsService>().GetChannel(VoiceStates[m.VoiceState.UserId].Model.ChannelId);
                    channel.ConnectedUsers.Remove(m.VoiceState.UserId);

                    if (m.VoiceState.ChannelId == null)
                    {
                        VoiceStates.Remove(m.VoiceState.UserId);

                        if (m.VoiceState.UserId == DiscordService.CurrentUser.Id)
                        {
                            DisconnectFromVoiceChannel();
                        }
                    }
                    else
                    {
                        VoiceStates[m.VoiceState.UserId].Model = m.VoiceState;
                        VoiceStates[m.VoiceState.UserId].UpateProperties();
                    }
                }
                else
                {
                    BindableVoiceUser voiceUser = new BindableVoiceUser(m.VoiceState);
                    VoiceStates.Add(m.VoiceState.UserId, voiceUser);
                }

                if (m.VoiceState.ChannelId != null)
                {
                    var channel = SimpleIoc.Default.GetInstance<IChannelsService>().GetChannel(m.VoiceState.ChannelId);
                    channel.ConnectedUsers.Add(m.VoiceState.UserId, VoiceStates[m.VoiceState.UserId]);
                }
            });

            Messenger.Default.Register<GatewayVoiceServerUpdateMessage>(this, m =>
            {
                ConnectToVoiceChannel(m.VoiceServer, VoiceStates[DiscordService.CurrentUser.Id].Model);
            });

            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                foreach (var guild in m.EventData.Guilds)
                {
                    if (guild.VoiceStates != null)
                    {
                        foreach (var state in guild.VoiceStates)
                        {
                            if (VoiceStates.ContainsKey(state.UserId))
                            {
                                VoiceStates[state.UserId].Model = state;
                            }
                            else
                            {
                                VoiceStates.Add(state.UserId, new BindableVoiceUser(state));
                            }
                        }
                    }
                }
            });
        }

        /// <inheritdoc/>
        public IDictionary<string, BindableVoiceUser> VoiceStates { get; } = new ConcurrentDictionary<string, BindableVoiceUser>();

        /// <inheritdoc/>
        public IAudioInService AudioInService => SimpleIoc.Default.GetInstance<IAudioInService>();

        /// <inheritdoc/>
        public IAudioOutService AudioOutService => SimpleIoc.Default.GetInstance<IAudioOutService>();

        private IDiscordService DiscordService => SimpleIoc.Default.GetInstance<IDiscordService>();

        /// <inheritdoc/>
        public async void ToggleDeafen()
        {
            AudioOutService.ToggleDeafen();
            var state = _voiceConnection._state;
            await DiscordService.Gateway.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, AudioInService.Muted, AudioOutService.Deafened);
        }

        /// <inheritdoc/>
        public async void ToggleMute()
        {
            AudioInService.ToggleMute();
            var state = _voiceConnection._state;
            await DiscordService.Gateway.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, AudioInService.Muted, AudioOutService.Deafened);
        }

        private async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            AudioOutService.CreateGraph();
            _voiceConnection = new VoiceConnection(data, state);
            _voiceConnection.VoiceDataRecieved += VoiceDataRecieved;
            _voiceConnection.Speak += Speak;
            await _voiceConnection.ConnectAsync();

            AudioInService.AudioQueued += InputRecieved;
            AudioInService.SpeakingChanged += SpeakingChanged;
            AudioInService.CreateGraph();
        }

        private void DisconnectFromVoiceChannel()
        {
            AudioInService.Dispose();
            AudioOutService.Dispose();
        }

        private void InputRecieved(object sender, float[] e)
        {
            _voiceConnection.SendVoiceData(e);
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
            _voiceConnection.SendSpeaking(e);
        }
    }
}