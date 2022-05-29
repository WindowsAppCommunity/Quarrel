// Quarrel © 2022

using Discord.API.Models.Enums.Channels;
using Quarrel.Client.Models.Base.Interfaces;

namespace Quarrel.Client.Models.Channels.Interfaces
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

        /// <summary>
        /// Gets the url of the channel.
        /// </summary>
        string Url { get; }
    }
}
