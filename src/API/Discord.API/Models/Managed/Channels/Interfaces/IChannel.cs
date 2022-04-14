// Quarrel © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Enums.Channels;

namespace Discord.API.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for channel.
    /// </summary>
    public interface IChannel : ISnowflakeItem
    {
        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets the channel type.
        /// </summary>
        ChannelType Type { get; }
    }
}
