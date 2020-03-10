// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.API.Channel;
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
    public sealed class VoiceService : IVoiceService
    {
        #region Public Properties

        public IAudioInService AudioInService { get; }

        public IAudioOutService AudioOutService { get; }

        private IDiscordService DiscordService { get; }

        #endregion

        #region Variables

        private VoiceConnection _VoiceConnection;

        public IDictionary<string, BindableVoiceUser> VoiceStates { get; } = new ConcurrentDictionary<string, BindableVoiceUser>();

        #endregion

        #region Constructor

        public VoiceService(IAudioInService audioInService, IAudioOutService audioOutService, IDiscordService discordService)
        {
            AudioInService = audioInService;
            AudioOutService = audioOutService;
            DiscordService = discordService;

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
                    } else
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

        #endregion

        #region Methods

        public async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            AudioOutService.CreateGraph();
            _VoiceConnection = new VoiceConnection(data, state);
            _VoiceConnection.VoiceDataRecieved += VoiceDataRecieved;
            _VoiceConnection.Speak += Speak;
            await _VoiceConnection.ConnectAsync();

            AudioInService.AudioQueued += InputRecieved;
            AudioInService.SpeakingChanged += SpeakingChanged;
            AudioInService.CreateGraph();
        }

        public void DisconnectFromVoiceChannel()
        {
            AudioInService.Dispose();
            AudioOutService.Dispose();
        }

        public async void ToggleDeafen()
        {
            AudioOutService.ToggleDeafen();
            var state = _VoiceConnection._state;
            await DiscordService.Gateway.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, AudioInService.Muted, AudioOutService.Deafened);
        }

        public async void ToggleMute()
        {
            AudioInService.ToggleMute();
            var state = _VoiceConnection._state;
            await DiscordService.Gateway.Gateway.VoiceStatusUpdate(state.GuildId, state.ChannelId, AudioInService.Muted, AudioOutService.Deafened);
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