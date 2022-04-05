// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Channels;

namespace Discord.API.Models.Channels
{
    /// <summary>
    /// A category channel managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public class CategoryChannel : Channel, ICategoryChannel
    {
        internal CategoryChannel(JsonChannel restChannel, ulong? guildId, DiscordClient context) :
            base(restChannel, context)
        {
            guildId = restChannel.GuildId ?? guildId;
            Guard.IsNotNull(restChannel.Position, nameof(restChannel.Position));
            Guard.IsNotNull(guildId, nameof(guildId));

            Position = restChannel.Position.Value;
            GuildId = guildId.Value;
        }

        /// <inheritdoc/>
        public ulong GuildId { get; private set; }

        /// <inheritdoc/>
        public int Position { get; private set; }

        internal override void UpdateFromRestChannel(JsonChannel jsonChannel)
        {
            base.UpdateFromRestChannel(jsonChannel);

            Position = jsonChannel.Position ?? Position;
            GuildId = jsonChannel.GuildId ?? GuildId;
        }

        internal override JsonChannel ToRestChannel()
        {
            JsonChannel restChannel = base.ToRestChannel();
            restChannel.Position = Position;
            restChannel.GuildId = GuildId;
            return restChannel;
        }
    }
}
