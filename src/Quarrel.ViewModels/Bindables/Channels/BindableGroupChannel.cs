// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Users;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of an <see cref="IGroupChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableGroupChannel : BindablePrivateChannel, IBindableMessageChannel
    {
        internal BindableGroupChannel(IDiscordService discordService, IDispatcherService dispatcherService, GroupChannel groupChannel) :
            base(discordService, dispatcherService, groupChannel)
        {
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

        /// <inheritdoc/>
        public IGroupChannel DirectChannel => (IGroupChannel)Channel;

        // TODO: Formatted names
        /// <inheritdoc/>
        public override string? Name => Channel.Name;

        public string IconUrl => $"https://cdn.discordapp.com/channel-icons/{Channel.Id}/{DirectChannel.Icon}.png";

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
