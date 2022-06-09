// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Enums.Channels;
using Discord.API.Models.Json.Channels;
using Quarrel.Client.Models.Base;
using Quarrel.Client.Models.Channels.Interfaces;

namespace Quarrel.Client.Models.Channels.Abstract
{
    /// <summary>
    /// A base class for a channel managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public abstract class Channel : SnowflakeItem, IChannel
    {
        internal Channel(JsonChannel restChannel, QuarrelClient context) :
            base(context)
        {
            Id = restChannel.Id;
            Name = restChannel.Name;
            Type = restChannel.Type;
        }

        /// <inheritdoc/>
        public string? Name { get; private set; }

        /// <inheritdoc/>
        public ChannelType Type { get; private set; }

        /// <inheritdoc/>
        public abstract string Url { get; }

        internal void UpdateFromJsonChannel(JsonChannel jsonChannel)
        {
            Guard.IsEqualTo(Id, jsonChannel.Id, nameof(Id));
            PrivateUpdateFromJsonChannel(jsonChannel);
        }

        internal virtual void PrivateUpdateFromJsonChannel(JsonChannel jsonChannel)
        {
            Name = jsonChannel.Name ?? Name;
            Type = jsonChannel.Type;
        }

        internal static Channel? FromJsonChannel(JsonChannel jsonChannel, QuarrelClient context, ulong? guildId = null)
        {
            return jsonChannel.Type switch
            {
                ChannelType.GuildText => new GuildTextChannel(jsonChannel, guildId, context),
                ChannelType.News => new GuildTextChannel(jsonChannel, guildId, context),
                ChannelType.DirectMessage => new DirectChannel(jsonChannel, context),
                ChannelType.GuildVoice => new VoiceChannel(jsonChannel, guildId, context),
                ChannelType.StageVoice => new VoiceChannel(jsonChannel, guildId, context),
                ChannelType.GroupDM => new GroupChannel(jsonChannel, context),
                ChannelType.Category => new CategoryChannel(jsonChannel, guildId, context),
                _ => null,
            };
        }

        internal virtual JsonChannel ToJsonChannel()
        {
            return new JsonChannel()
            {
                Id = Id,
                Name = Name,
                Type = Type,
            };
        }
    }
}
