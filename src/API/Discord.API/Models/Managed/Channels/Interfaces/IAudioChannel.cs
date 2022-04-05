// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for channels voice channels or channels with calling.
    /// </summary>
    internal interface IAudioChannel : IChannel
    {
        /// <summary>
        /// The region of the voice server.
        /// </summary>
        string? RTCRegion { get; }
    }
}
