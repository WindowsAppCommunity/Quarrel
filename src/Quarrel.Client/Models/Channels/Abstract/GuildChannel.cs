// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Permissions;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Permissions;

namespace Quarrel.Client.Models.Channels.Abstract
{
    /// <summary>
    /// The base class for channels in a guild.
    /// </summary>
    public abstract class GuildChannel : Channel, IGuildChannel
    {
        internal GuildChannel(JsonChannel restChannel, ulong? guildId, QuarrelClient context) :
            base(restChannel, context)
        {
            guildId = restChannel.GuildId ?? guildId;
            Guard.IsNotNull(restChannel.Position, nameof(restChannel.Position));
            Guard.IsNotNull(guildId, nameof(guildId));

            Position = restChannel.Position.Value;
            GuildId = guildId.Value;

            if (restChannel.PermissionOverwrites is not null)
            {
                PermissionOverwrites = CreateOverwrites(restChannel.PermissionOverwrites);
            }
        }
        
        /// <inheritdoc/>
        public int Position { get; private set; }

        /// <inheritdoc/>
        public ulong GuildId { get; private set; }

        /// <inheritdoc/>
        public override string Url => $"https://discord.com/channels/{GuildId}/{Id}";

        /// <summary>
        /// Gets the permission overwrites for the channel.
        /// </summary>
        public PermissionOverwrite[]? PermissionOverwrites { get; private set; }

        internal override void PrivateUpdateFromJsonChannel(JsonChannel jsonChannel)
        {
            Position = jsonChannel.Position ?? Position;
            GuildId = jsonChannel.GuildId ?? GuildId;

            if (jsonChannel.PermissionOverwrites is not null)
            {
                PermissionOverwrites = CreateOverwrites(jsonChannel.PermissionOverwrites);
            }

            base.PrivateUpdateFromJsonChannel(jsonChannel);
        }

        internal override JsonChannel ToJsonChannel()
        {
            JsonChannel restChannel = base.ToJsonChannel();
            restChannel.Position = Position;
            restChannel.GuildId = GuildId;
            return restChannel;
        }

        private PermissionOverwrite[] CreateOverwrites(JsonOverwrite[] jsonOverwrites)
        {
            PermissionOverwrite[] overwrites = new PermissionOverwrite[jsonOverwrites.Length];
            for (int i = 0; i < overwrites.Length; i++)
            {
                overwrites[i] = new PermissionOverwrite(jsonOverwrites[i]);
            }

            return overwrites;
        }
    }
}
