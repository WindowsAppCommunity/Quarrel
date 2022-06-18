// Quarrel © 2022

using Quarrel.Client.Models.Voice;

namespace Quarrel.Messages.Discord.Voice
{
    /// <summary>
    /// A message sent when a the current user's voice state is updated.
    /// </summary>
    public class MyVoiceStateUpdatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyVoiceStateUpdatedMessage"/> class.
        /// </summary>
        /// <param name="voiceState">The voice state updated.</param>
        public MyVoiceStateUpdatedMessage(VoiceState voiceState)
        {
            VoiceState = voiceState;
        }

        /// <summary>
        /// Gets the current user's new voice state.
        /// </summary>
        public VoiceState VoiceState { get; }
    }
}