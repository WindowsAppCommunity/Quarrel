// Quarrel © 2022

namespace Quarrel.Client.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for channels voice channels or channels with calling.
    /// </summary>
    public interface IAudioChannel : IChannel
    {
        /// <summary>
        /// The region of the voice server.
        /// </summary>
        string? RTCRegion { get; }
    }
}
