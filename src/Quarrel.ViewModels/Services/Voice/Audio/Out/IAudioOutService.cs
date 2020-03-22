// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Services.Voice.Audio.Out
{
    /// <summary>
    /// A <see langword="class"/> that manages incoming audio.
    /// </summary>
    public interface IAudioOutService : IAudioService
    {
        /// <summary>
        /// Gets a value indicating whether or not the user is deafened.
        /// </summary>
        bool Deafened { get; }

        /// <summary>
        /// Adds a frame of audio to the the queue.
        /// </summary>
        /// <param name="framedata">PCM data to add.</param>
        /// <param name="samples">samples of audio.</param>
        unsafe void AddFrame(float[] framedata, uint samples);

        /// <summary>
        /// Deafens output to the user.
        /// </summary>
        void Deafen();

        /// <summary>
        /// Undeafens output to the user.
        /// </summary>
        void Undeafen();

        /// <summary>
        /// Toggles if output to the user is deafend.
        /// </summary>
        void ToggleDeafen();
    }
}
