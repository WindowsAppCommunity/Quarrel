// Quarrel © 2022

namespace Quarrel.Client.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for voice channels in a guild.
    /// </summary>
    internal interface IGuildVoiceChannel : INestedChannel, IAudioChannel
    {
        /// <summary>
        /// The bitrate of the channel.
        /// </summary>
        int Bitrate { get; }

        /// <summary>
        /// The max number of users allowed in the channel.
        /// </summary>
        int? UserLimit { get; }
    }
}
