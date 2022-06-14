// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Channels.VoiceChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableVoiceChannel : BindableGuildChannel, IBindableAudioChannel
    {
        internal BindableVoiceChannel(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            VoiceChannel channel,
            GuildMember selfMember,
            BindableCategoryChannel? parent = null) :
            base(messenger, clipboardService, discordService, dispatcherService, channel, selfMember, parent)
        {
            JoinCallCommand = new RelayCommand(JoinCall);
        }
        
        /// <inheritdoc/>
        public override bool IsTextChannel => false;

        /// <inheritdoc/>
        public override bool IsAccessible => Permissions.Connect;

        /// <inheritdoc/>
        public IAudioChannel AudioChannel => (IAudioChannel)Channel;

        /// <inheritdoc/>
        public VoiceChannel VoiceChannel => (VoiceChannel)Channel;

        /// <inheritdoc/>
        public RelayCommand JoinCallCommand { get; }

        public async void JoinCall()
        {
            await _discordService.JoinCall(Id, GuildId);
        }
    }
}
