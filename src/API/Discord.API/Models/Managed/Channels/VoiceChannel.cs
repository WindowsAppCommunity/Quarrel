// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Channels;

namespace Discord.API.Models.Channels
{
    public class VoiceChannel : Channel, IGuildVoiceChannel
    {
        internal VoiceChannel(JsonChannel restChannel, ulong? guildId, DiscordClient context) :
            base(restChannel, context)
        {
            guildId = restChannel.GuildId ?? guildId;
            Guard.IsNotNull(restChannel.Bitrate, nameof(restChannel.Bitrate));
            Guard.IsNotNull(restChannel.CategoryId, nameof(restChannel.CategoryId));
            Guard.IsNotNull(restChannel.Position, nameof(restChannel.Position));
            Guard.IsNotNull(guildId, nameof(guildId));

            Bitrate = restChannel.Bitrate.Value;
            UserLimit = restChannel.UserLimit;
            CategoryId = restChannel.CategoryId.Value;
            Position = restChannel.Position.Value;
            GuildId = guildId.Value;
            RTCRegion = restChannel.RTCRegion;
        }

        public int Bitrate { get; private set; }

        public int? UserLimit { get; private set; }

        public ulong CategoryId { get; private set; }

        public int Position { get; private set; }

        public ulong GuildId { get; private set; }

        public string? RTCRegion { get; private set; }

        internal override JsonChannel ToRestChannel()
        {
            JsonChannel restChannel = base.ToRestChannel();
            restChannel.Bitrate = Bitrate;
            restChannel.UserLimit = UserLimit;
            restChannel.CategoryId = CategoryId;
            restChannel.Position = Position;
            restChannel.GuildId = GuildId;
            restChannel.RTCRegion = RTCRegion;
            return restChannel;
        }
    }
}
