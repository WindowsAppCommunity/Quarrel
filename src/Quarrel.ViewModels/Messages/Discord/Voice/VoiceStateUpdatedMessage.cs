// Quarrel © 2022

using Quarrel.Client.Models.Voice;

namespace Quarrel.Messages.Discord.Voice
{
    /// <summary>
    /// A message sent when a voice state is updated.
    /// </summary>
    public class VoiceStateUpdatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceStateUpdatedMessage"/> class.
        /// </summary>
        /// <param name="voiceState">The voice state updated.</param>
        public VoiceStateUpdatedMessage(VoiceState voiceState)
        {
            VoiceState = voiceState;
        }

        /// <summary>
        /// Gets the message created.
        /// </summary>
        public VoiceState VoiceState { get; }
    }
}
