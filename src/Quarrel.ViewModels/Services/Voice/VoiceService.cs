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
using Quarrel.ViewModels.Services.DispatcherHelper;
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
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (VoiceStates.ContainsKey(m.VoiceState.UserId))
                    {
                        var oldChannel = ChannelsService.GetChannel(VoiceStates[m.VoiceState.UserId].Model.ChannelId);
                        oldChannel?.ConnectedUsers.Remove(m.VoiceState.UserId);

                        if (m.VoiceState.ChannelId == null)
                        {
                            VoiceStates.Remove(m.VoiceState.UserId);

                            if (m.VoiceState.UserId == DiscordService.CurrentUser.Id)
                            {
                                // Tell webrtc manager.
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
            });

            Messenger.Default.Register<SpeakMessage>(this, e =>
            {
                if (e.EventData.UserId != null && VoiceStates.ContainsKey(e.EventData.UserId))
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() => { VoiceStates[e.EventData.UserId].Speaking = e.EventData.Speaking > 0; });
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

        private IDiscordService DiscordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();

        private IChannelsService ChannelsService { get; } = SimpleIoc.Default.GetInstance<IChannelsService>();

        private IDispatcherHelper DispatcherHelper { get; } = SimpleIoc.Default.GetInstance<IDispatcherHelper>();

        private IWebrtcManager WebrtcManager { get; } = SimpleIoc.Default.GetInstance<IWebrtcManager>();

        /// <inheritdoc/>
        public async void ToggleDeafen()
        {
            var state = _voiceConnection._state;
            await DiscordService.Gateway.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, state.IsMuted, !state.IsDeafened);
        }

        /// <inheritdoc/>
        public async void ToggleMute()
        {
            var state = _voiceConnection._state;
            await DiscordService.Gateway.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, !state.IsMuted, state.IsDeafened);
        }

        private async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            _voiceConnection = new VoiceConnection(data, state, WebrtcManager);

            _voiceConnection.Speak += Speak;
            await _voiceConnection.ConnectAsync();
        }

        private void DisconnectFromVoiceChannel()
        {
        }

        private void InputRecieved(object sender, float[] e)
        {
            _voiceConnection.SendVoiceData(e);
        }

        private void VoiceDataRecieved(object sender, VoiceConnectionEventArgs<VoiceData> e)
        {
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