// Quarrel © 2022

using Quarrel.Client.Models.Voice;

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

        VoiceState[] GetVoiceStates();

        void AddVoiceMember(ulong userId);

        void RemoveVoiceMember(ulong userId);
    }
}
