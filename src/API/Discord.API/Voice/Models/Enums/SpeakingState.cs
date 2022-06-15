// Quarrel © 2022

using System;

namespace Discord.API.Voice.Models.Enums
{
    /// <summary>
    /// An enum for a user's speaking state.
    /// </summary>
    [Flags]
    public enum SpeakingState : int
    {
        /// <summary>
        /// The user is speaking through their microphone.
        /// </summary>
        Microphone = 0x1,
        /// <summary>
        /// The user is sharing audio from their PC.
        /// </summary>
        SoundShare = 0x2,

        /// <summary>
        /// The user is speaking with priority voice.
        /// </summary>
        Priority = 0x4,
    }
}
