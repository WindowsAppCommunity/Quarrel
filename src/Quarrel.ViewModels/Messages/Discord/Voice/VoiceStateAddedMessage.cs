// Quarrel © 2022

using Quarrel.Client.Models.Voice;

namespace Quarrel.Messages.Discord.Voice
{
    /// <summary>
    /// A message sent when any user joins a voice channel.
    /// </summary>
    public class VoiceStateAddedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceStateAddedMessage"/> class.
        /// </summary>
        /// <param name="voiceState">The voice state of the added user.</param>
        public VoiceStateAddedMessage(VoiceState voiceState)
        {
            VoiceState = voiceState;
        }

        /// <summary>
        /// Gets the voice state updated.
        /// </summary>
        public VoiceState VoiceState { get; }
    }
}
