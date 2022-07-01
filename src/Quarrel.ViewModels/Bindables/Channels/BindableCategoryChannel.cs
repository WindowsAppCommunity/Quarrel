// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Client;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Users;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of a <see cref="CategoryChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableCategoryChannel : BindableGuildChannel
    {
        internal BindableCategoryChannel(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService,
            CategoryChannel channel,
            GuildMember selfMember) :
            base(messenger, clipboardService, discordService, quarrelClient, dispatcherService, channel, selfMember)
        {
        }

        /// <inheritdoc/>
        public override bool IsAccessible => true;
    }
}
