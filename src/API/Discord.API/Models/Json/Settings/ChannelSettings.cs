// Quarrel © 2022

namespace Discord.API.Models.Json.Settings
{
    internal class ChannelSettings
    {
        public ChannelSettings(JsonChannelOverride channelOverride)
        {
            ChannelId = channelOverride.ChannelId;
            Muted = channelOverride.Muted;
        }

        public ulong ChannelId { get; }

        public bool Muted { get; }
    }
}
