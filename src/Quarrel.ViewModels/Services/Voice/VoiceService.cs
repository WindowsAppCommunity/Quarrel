// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using DiscordAPI.Voice;
using DiscordAPI.Voice.DownstreamEvents;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Voice;
using Quarrel.ViewModels.Services.Rest;
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

        public IDictionary<string, VoiceState> VoiceStates { get; } = new ConcurrentDictionary<string, VoiceState>();

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
                    if (m.VoiceState.UserId == DiscordService.CurrentUser.Id &&
                        m.VoiceState.ChannelId == null)
                    {
                        VoiceStates.Remove(m.VoiceState.UserId);
                        DisconnectFromVoiceChannel();
                    }

                    VoiceStates[m.VoiceState.UserId] = m.VoiceState;
                }
                else
                {
                    VoiceStates.Add(m.VoiceState.UserId, m.VoiceState);
                }
            });

            Messenger.Default.Register<GatewayVoiceServerUpdateMessage>(this, m => 
            {
                ConnectToVoiceChannel(m.VoiceServer, VoiceStates[DiscordService.CurrentUser.Id]);
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