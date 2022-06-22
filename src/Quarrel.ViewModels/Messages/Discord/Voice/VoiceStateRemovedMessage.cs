// Quarrel © 2022

using Quarrel.Client.Models.Voice;

namespace Quarrel.Messages.Discord.Voice
{
    /// <summary>
    /// A message sent when any user leaves a voice channel.
    /// </summary>
    public class VoiceStateRemovedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceStateRemovedMessage"/> class.
        /// </summary>
        /// <param name="voiceState">The voice state of the removed user.</param>
        public VoiceStateRemovedMessage(VoiceState voiceState)
        {
            VoiceState = voiceState;
        }

        /// <summary>
        /// Gets the voice state updated.
        /// </summary>
        public VoiceState VoiceState { get; }
    }
}
