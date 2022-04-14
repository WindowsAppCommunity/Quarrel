﻿// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Permissions;

namespace Discord.API.Models.Managed.Channels.Abstract
{
    public abstract class GuildChannel : Channel, IGuildChannel
    {
        internal GuildChannel(JsonChannel restChannel, ulong? guildId, DiscordClient context) :
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

        public int Position { get; private set; }

        /// <inheritdoc/>
        public ulong GuildId { get; private set; }

        public PermissionOverwrite[] PermissionOverwrites { get; private set; }

        internal override void UpdateFromRestChannel(JsonChannel jsonChannel)
        {
            base.UpdateFromRestChannel(jsonChannel);

            Position = jsonChannel.Position ?? Position;
            GuildId = jsonChannel.GuildId ?? GuildId;

            if (jsonChannel.PermissionOverwrites is not null)
            {
                PermissionOverwrites = CreateOverwrites(jsonChannel.PermissionOverwrites);
            }
        }

        internal override JsonChannel ToRestChannel()
        {
            JsonChannel restChannel = base.ToRestChannel();
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
