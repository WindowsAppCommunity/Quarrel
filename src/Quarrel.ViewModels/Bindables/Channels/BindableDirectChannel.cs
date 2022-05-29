// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Users;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of an <see cref="IDirectChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableDirectChannel : BindablePrivateChannel, IBindableMessageChannel
    {
        internal BindableDirectChannel(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            DirectChannel directChannel) :
            base(messenger, clipboardService, discordService, dispatcherService, directChannel)
        {
            BindableUser? user = _discordService.GetUser(DirectChannel.RecipientId);
            Guard.IsNotNull(user);
            Recipient = user;
        }

        /// <inheritdoc/>
        public IDirectChannel DirectChannel => (IDirectChannel)Channel;

        /// <inheritdoc/>
        public override string? Name => Recipient.User.Username;

        /// <summary>
        /// Gets the recipient of the direct messages as a <see cref="BindableUser"/>.
        /// </summary>
        public BindableUser Recipient { get; }
    }
}
