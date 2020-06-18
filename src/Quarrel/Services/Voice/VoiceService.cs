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
        private readonly ISettingsService _settingsService;

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
            IGatewayService gatewayService,
            ISettingsService settingsService)
        {
            _channelsService = channelsService;
            _discordService = discordService;
            _dispatcherHelper = dispatcherHelper;
            _gatewayService = gatewayService;
            _settingsService = settingsService;
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

            Messenger.Default.Register<SettingChangedMessage<string>>(this, async (m) =>
            {
                // Todo: this
                switch (m.Key)
                {
                    case SettingKeys.InputDevice:
                        SetInputDevice(m.Value);
                        break;
                    case SettingKeys.OutputDevice:
                        SetOutputDevice(m.Value);
                        break;
                }
            });
        }

        /// <inheritdoc/>
        public IDictionary<string, VoiceState> VoiceStates { get; } = new ConcurrentDictionary<string, VoiceState>();

        /// <inheritdoc/>
        public async void ToggleDeafen()
        {
            var state = VoiceStates[_discordService.CurrentUser.Id];
            state.SelfDeaf = !state.SelfDeaf;
            var message = new ValueSet
            {
                ["type"] = "voiceStateUpdate",
                ["state"] = JsonConvert.SerializeObject(state),
            };
            AppServiceResponse response = await SendServiceMessage(message);

            await _gatewayService.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, state.SelfMute, state.SelfDeaf);
        }

        /// <inheritdoc/>
        public async void ToggleMute()
        {
            var state = VoiceStates[_discordService.CurrentUser.Id];
            state.SelfMute = !state.SelfMute;
            var message = new ValueSet
            {
                ["type"] = "voiceStateUpdate",
                ["state"] = JsonConvert.SerializeObject(state),
            };
            AppServiceResponse response = await SendServiceMessage(message);

            await _gatewayService.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, state.SelfMute, state.SelfDeaf);
        }

        public event EventHandler<IList<float>> AudioInData;
        public event EventHandler<IList<float>> AudioOutData;

        private void SetInputDevice(string id)
        {
            Task.Run(async() =>
            {
                var message = new ValueSet
                {
                    ["type"] = "inputDevice",
                    ["id"] = id,
                };
                AppServiceResponse response = await SendServiceMessage(message);

            });
        }

        private void SetOutputDevice(string id)
        {
            Task.Run(async () =>
            {
                var message = new ValueSet
                {
                    ["type"] = "outputDevice",
                    ["id"] = id,
                };
                AppServiceResponse response = await SendServiceMessage(message);
            });
        }

        private async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            var message = new ValueSet
            {
                ["type"] = "connect",
                ["config"] = JsonConvert.SerializeObject(data),
                ["state"] = JsonConvert.SerializeObject(state),
                ["inputId"] = _settingsService.Roaming.GetValue<string>(SettingKeys.InputDevice),
                ["outputId"] = _settingsService.Roaming.GetValue<string>(SettingKeys.OutputDevice),
            };
            AppServiceResponse response = await SendServiceMessage(message);
        }

        private async Task<bool> CreateAppService()
        {
            _serviceConnection = new AppServiceConnection
            {
                AppServiceName = "com.Quarrel.voip",
                PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName,
            };

            var status = await _serviceConnection.OpenAsync();

            _serviceConnection.RequestReceived += OnRequestReceived;
            _serviceConnection.ServiceClosed += OnServiceClosed;

            return status == AppServiceConnectionStatus.Success;
        }

        private void OnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            _serviceConnection = null;
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral deferral = args.GetDeferral();
            ValueSet message = args.Request.Message;

            if (message == null || !message.ContainsKey("type"))
            {

                return;
            }

            switch (message["type"])
            {
                case "speaking":
                    Speak speak = JsonConvert.DeserializeObject<Speak>(message["payload"] as string);
                    Messenger.Default.Send(new SpeakMessage(speak));
                    break;
                case "audioInData":
                    AudioInData?.Invoke(this, message["data"] as float[]);
                    break;
                case "audioOutData":
                    AudioOutData?.Invoke(this, message["data"] as float[]);
                    break;
            }
            deferral.Complete();
        }

        private async void DisconnectFromVoiceChannel()
        {
            var message = new ValueSet
            {
                ["type"] = "disconnect",
            };

            AppServiceResponse response = await SendServiceMessage(message);
        }

        private async Task<AppServiceResponse> SendServiceMessage(ValueSet message)
        {
            if (_serviceConnection == null)
            {
                if (!await CreateAppService())
                {
                    return null;
                }
            }

            return await _serviceConnection.SendMessageAsync(message);
        }
    }
}