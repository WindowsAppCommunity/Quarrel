// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables;
using System.Collections.Generic;

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
        IDictionary<string, BindableVoiceUser> VoiceStates { get; }

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
