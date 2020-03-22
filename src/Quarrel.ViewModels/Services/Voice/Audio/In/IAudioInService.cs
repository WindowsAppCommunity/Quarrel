// Copyright (c) Quarrel. All rights reserved.

using System;

namespace Quarrel.ViewModels.Services.Voice.Audio.In
{
    /// <summary>
    /// A <see langword="class"/> that manages incoming audio.
    /// </summary>
    public interface IAudioInService : IAudioService
    {
        /// <summary>
        /// Fired when the user begins or stops speaking.
        /// </summary>
        event EventHandler<int> SpeakingChanged;

        /// <summary>
        /// Gets a value indicating whether or not the user is muted.
        /// </summary>
        bool Muted { get; }

        /// <summary>
        /// Mutes input from the user.
        /// </summary>
        void Mute();

        /// <summary>
        /// Unmutes input from the user.
        /// </summary>
        void Unmute();

        /// <summary>
        /// Toggles if input from the user is muted.
        /// </summary>
        void ToggleMute();
    }
}
