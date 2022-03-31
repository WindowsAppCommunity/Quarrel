// Adam Dernis © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Enums.Channels;

namespace Discord.API.Models.Channels.Interfaces
{
    public interface IChannel : ISnowflakeItem
    {
        string? Name { get; }

        ChannelType Type { get; }
    }
}
