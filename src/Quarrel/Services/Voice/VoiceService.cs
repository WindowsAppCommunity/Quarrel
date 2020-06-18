// Copyright (c) Quarrel. All rights reserved.

using System;
using DiscordAPI.Models;
using DiscordAPI.Voice;
using DiscordAPI.Voice.DownstreamEvents;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Voice;
using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.Services.Gateway;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Newtonsoft.Json;
using Quarrel.ViewModels.Messages.Services.Settings;
using Quarrel.ViewModels.Services.Settings;

namespace Quarrel.ViewModels.Services.Voice
{
    /// <summary>
    /// Manages all voice state data.
    /// </summary>
    public sealed class VoiceService : IVoiceService
    {
        private readonly IChannelsService _channelsService;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly IGatewayService _gatewayService;
        private readonly IWebrtcManager _webrtcManager;
        private VoiceConnection _voiceConnection;
        private AppServiceConnection _serviceConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceService"/> class.
        /// </summary>
        /// <param name="channelsService">The app's channel service.</param>
        /// <param name="discordService">The app's discord service.</param>
        /// <param name="dispatcherHelper">The app's dispatcher helper.</param>
        /// <param name="gatewayService">The app's gateway service.</param>
        /// <param name="webrtcManager">The app's webrtc manager.</param>
        public VoiceService(
            IChannelsService channelsService,
            IDiscordService discordService,
            IDispatcherHelper dispatcherHelper,
            IGatewayService gatewayService)
        {
            _channelsService = channelsService;
            _discordService = discordService;
            _dispatcherHelper = dispatcherHelper;
            _gatewayService = gatewayService;
            _ = CreateAppService();
            Messenger.Default.Register<GatewayVoiceStateUpdateMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (VoiceStates.ContainsKey(m.VoiceState.UserId))
                    {
                        var oldChannel = _channelsService.GetChannel(VoiceStates[m.VoiceState.UserId].ChannelId);
                        oldChannel?.ConnectedUsers.Remove(m.VoiceState.UserId);

                        if (m.VoiceState.ChannelId == null)
                        {
                            if (m.VoiceState.UserId == _discordService.CurrentUser.Id)
                            {
                                DisconnectFromVoiceChannel();
                            }
                            else
                            {
                                VoiceStates.Remove(m.VoiceState.UserId);
                            }
                        }
                        else
                        {
                            VoiceStates[m.VoiceState.UserId] = m.VoiceState;
                        }
                    }
                    else
                    {
                        VoiceStates[m.VoiceState.UserId] = m.VoiceState;
                    }
                });
            });

            Messenger.Default.Register<GatewayVoiceServerUpdateMessage>(this, m =>
            {
                if (!VoiceStates.ContainsKey(_discordService.CurrentUser.Id))
                {
                    VoiceStates.Add(
                        _discordService.CurrentUser.Id,
                        new VoiceState()
                        {
                            ChannelId = m.VoiceServer.ChannelId,
                            GuildId = m.VoiceServer.GuildId,
                            UserId = _discordService.CurrentUser.Id,
                        });
                }

                ConnectToVoiceChannel(m.VoiceServer, VoiceStates[_discordService.CurrentUser.Id]);
            });

            Messenger.Default.Register<SettingChangedMessage<string>>(this, m =>
            {
                // Todo: this
                switch (m.Key)
                {
                    case SettingKeys.InputDevice:
                        //_webrtcManager.SetRecordingDevice(m.Value);
                        break;
                    case SettingKeys.OutputDevice:
                        //_webrtcManager.SetPlaybackDevice(m.Value);
                        break;
                }
            });
        }

        /// <inheritdoc/>
        public IDictionary<string, VoiceState> VoiceStates { get; } = new ConcurrentDictionary<string, VoiceState>();

        /// <inheritdoc/>
        public async void ToggleDeafen()
        {
            if (_voiceConnection != null)
            {
                var state = _voiceConnection._state;
                state.SelfDeaf = !state.SelfDeaf;
                await _gatewayService.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, state.SelfMute, state.SelfDeaf);
            }
        }

        /// <inheritdoc/>
        public async void ToggleMute()
        {
            if (_voiceConnection != null)
            {
                var state = _voiceConnection._state;
                state.SelfMute = !state.SelfMute;
                await _gatewayService.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, state.SelfMute, state.SelfDeaf);
            }
        }

        private async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            if (_serviceConnection != null)
            {
                if (!await CreateAppService())
                {
                    return;
                }
            }

            var message = new ValueSet
            {
                ["type"] = "connect",
                ["config"] = JsonConvert.SerializeObject(data),
                ["state"] = JsonConvert.SerializeObject(state),
            };
            AppServiceResponse response = await _serviceConnection.SendMessageAsync(message);
            return;
            _voiceConnection = new VoiceConnection(data, state, _webrtcManager);
            _voiceConnection.Speak += Speak;
            await _voiceConnection.ConnectAsync();
        }

        private async Task<bool> CreateAppService()
        {
            _serviceConnection = new AppServiceConnection
            {
                AppServiceName = "com.Quarrel.voip",
                PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName,
            };

            var status = await _serviceConnection.OpenAsync();
            return status == AppServiceConnectionStatus.Success;
        }

        private void DisconnectFromVoiceChannel()
        {
            // Todo: this
            //_webrtcManager.Destroy();
        }

        private void InputRecieved(object sender, float[] e)
        {
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