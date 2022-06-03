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
using Quarrel.Services.Localization;
using System.Linq;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of an <see cref="IGroupChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableGroupChannel : BindablePrivateChannel, IBindableMessageChannel
    {
        private readonly ILocalizationService _localizationService;

        internal BindableGroupChannel(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            ILocalizationService localizationService,
            IDispatcherService dispatcherService,
            GroupChannel groupChannel) :
            base(messenger, clipboardService, discordService, dispatcherService, groupChannel)
        {
            _localizationService = localizationService;

            Guard.IsNotNull(groupChannel.Recipients);
            Recipients = new BindableUser[groupChannel.Recipients.Length];
            int i = 0;
            foreach (var recipient in groupChannel.Recipients)
            {
                BindableUser? user = _discordService.GetUser(recipient.Id);
                Guard.IsNotNull(user);
                Recipients[i] = user;
                i++;
            }
        }

        /// <summary>
        /// Gets the wrapped channel as a <see cref="IGroupChannel"/>.
        /// </summary>
        public IGroupChannel GroupChannel => (IGroupChannel)Channel;

        /// <inheritdoc/>
        public override string? Name => Channel.Name ?? _localizationService.CommaList(Recipients.Select(x => x.User.Username).ToArray());

        /// <summary>
        /// Gets the icon url of the group channel.
        /// </summary>
        public string? IconUrl => GroupChannel.Icon is null ? null : $"https://cdn.discordapp.com/channel-icons/{Channel.Id}/{GroupChannel.Icon}.png";

        /// <summary>
        /// Gets the recipients of the group channel as a <see cref="BindableUser"/> array.
        /// </summary>
        public BindableUser[] Recipients { get; }

        /// <summary>
        /// Gets the number of members in the group channel.
        /// </summary>
        public int MemberCount => Recipients.Length + 1;
    }
}
