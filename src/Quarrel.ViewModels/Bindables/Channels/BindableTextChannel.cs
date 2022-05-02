// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of a <see cref="GuildTextChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableTextChannel : BindableGuildChannel, IBindableMessageChannel
    {
        internal BindableTextChannel(
            IMessenger messenger,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            GuildTextChannel channel,
            GuildMember selfMember,
            BindableCategoryChannel? parent = null) :
            base(messenger, discordService, dispatcherService, channel, selfMember, parent)
        {
        }

        /// <inheritdoc/>
        public override bool IsTextChannel => true;

        /// <inheritdoc/>
        public override bool IsAccessible => Permissions.ReadMessages;
        
        /// <inheritdoc/>
        public IMessageChannel MessageChannel => (IMessageChannel)Channel;
        
        /// <inheritdoc/>
        public GuildTextChannel TextChannel => (GuildTextChannel)Channel;

        /// <inheritdoc/>
        protected override void AckUpdate()
        {
            base.AckUpdate();
            OnPropertyChanged(nameof(MessageChannel));
        }
    }
}
