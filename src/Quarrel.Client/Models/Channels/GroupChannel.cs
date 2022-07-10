// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Settings;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;
using Quarrel.Client.Models.Users.Interfaces;
using System.Linq;

namespace Quarrel.Client.Models.Channels
{
    /// <summary>
    /// A group dm channel managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class GroupChannel : PrivateChannel, IGroupChannel
    {
        internal GroupChannel(JsonChannel restChannel, ChannelSettings? settings, QuarrelClient context) :
            base(restChannel, settings, context)
        {
            Guard.IsNotNull(restChannel.OwnerId, nameof(restChannel.OwnerId));
            Guard.IsNotNull(restChannel.Recipients, nameof(restChannel.Recipients));

            OwnerId = restChannel.OwnerId.Value;

            Recipients = restChannel.Recipients.Select(x => new User(x, context)).ToArray();
            Icon = restChannel.Icon;
        }

        /// <inheritdoc/>
        public ulong OwnerId { get; private set; }

        /// <inheritdoc/>
        public User[] Recipients { get; private set; }

        /// <inheritdoc/>
        public string? Icon { get; private set; }

        IUser[] IGroupChannel.Recipients => Recipients;

        internal override JsonChannel ToJsonChannel()
        {
            JsonChannel restChannel = base.ToJsonChannel();
            restChannel.OwnerId = OwnerId;
            restChannel.RTCRegion = RTCRegion;
            restChannel.Recipients = Recipients.Select(x => x.ToRestUser()).ToArray();
            return restChannel;
        }
    }
}
