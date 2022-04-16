// Quarrel © 2022

namespace Quarrel.Client.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for text channels in a guild.
    /// </summary>
    internal interface IGuildTextChannel : IMessageChannel, INestedChannel
    {
        /// <summary>
        /// The description of the channel.
        /// </summary>
        string? Topic { get; }

        /// <summary>
        /// Whether or not the channel is marked NSFW.
        /// </summary>
        bool? IsNSFW { get; }

        /// <summary>
        /// The duration of the slow mode delay to avoid spam.
        /// </summary>
        int? SlowModeDelay { get; }
    }
}
