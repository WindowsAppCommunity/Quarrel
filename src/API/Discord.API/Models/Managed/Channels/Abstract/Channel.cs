// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Base;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Enums.Channels;
using Discord.API.Models.Json.Channels;

namespace Discord.API.Models.Channels.Abstract
{
    /// <summary>
    /// A base class for a channel managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public abstract class Channel : SnowflakeItem, IChannel
    {
        internal Channel(JsonChannel restChannel, DiscordClient context) :
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

        internal virtual void UpdateFromRestChannel(JsonChannel jsonChannel)
        {
            Guard.IsEqualTo(Id, jsonChannel.Id, nameof(Id));

            Name = jsonChannel.Name ?? Name;
            Type = jsonChannel.Type;
        }

        internal static Channel? FromRestChannel(JsonChannel jsonChannel, DiscordClient context, ulong? guildId = null)
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

        internal virtual JsonChannel ToRestChannel()
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
