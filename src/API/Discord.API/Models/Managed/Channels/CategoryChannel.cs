// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Channels;

namespace Discord.API.Models.Channels
{
    public class CategoryChannel : Channel, ICategoryChannel
    {
        internal CategoryChannel(JsonChannel restChannel) : base(restChannel)
        {
            Guard.IsNotNull(restChannel.Position, nameof(restChannel.Position));
            Guard.IsNotNull(restChannel.GuildId, nameof(restChannel.GuildId));

            Position = restChannel.Position.Value;
            GuildId = restChannel.GuildId.Value;
        }

        public int Position { get; private set; }

        public ulong GuildId { get; private set; }

        internal override JsonChannel ToRestChannel()
        {
            JsonChannel restChannel = base.ToRestChannel();
            restChannel.Position = Position;
            restChannel.GuildId = GuildId;
            return restChannel;
        }
    }
}
