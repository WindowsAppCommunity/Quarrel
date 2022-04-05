// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for channels that belong to a guild.
    /// </summary>
    internal interface IGuildChannel : IChannel
    {
        /// <summary>
        /// The position with in the guild of the channel.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// The ID of the guild the channel belongs to.
        /// </summary>
        ulong GuildId { get; }
    }
}
