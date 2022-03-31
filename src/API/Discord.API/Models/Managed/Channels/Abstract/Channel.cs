// Adam Dernis © 2022

using Discord.API.Models.Base;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Enums.Channels;
using Discord.API.Models.Json.Channels;

namespace Discord.API.Models.Channels.Abstract
{
    public abstract class Channel : SnowflakeItem, IChannel
    {
        internal Channel(JsonChannel restChannel)
        {
            Name = restChannel.Name;
            Type = restChannel.Type;
        }

        public string? Name { get; private set; }

        public ChannelType Type { get; private set; }

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
