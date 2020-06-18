// Copyright (c) Quarrel. All rights reserved.

using System;
using Quarrel.ViewModels.Models.Bindables.Channels;
using System.Collections.Generic;
using DiscordAPI.Models;

namespace Quarrel.ViewModels.Services.Voice
{
    /// <summary>
    /// Manages all voice state data.
    /// </summary>
    public interface IVoiceService
    {
        /// <summary>
        /// Gets a hashed collection of guild member's voice states by user id.
        /// </summary>
        IDictionary<string, VoiceState> VoiceStates { get; }

        /// <summary>
        /// Fired when input data is recieved.
        /// </summary>
        event EventHandler<IList<float>> AudioInData;

        /// <summary>
        /// Fired when output data is recieved.
        /// </summary>
        event EventHandler<IList<float>> AudioOutData;

        /// <summary>
        /// Toggles if the user is deafend.
        /// </summary>
        void ToggleDeafen();

        /// <summary>
        /// Toggles if the user is muted.
        /// </summary>
        void ToggleMute();
    }
}
