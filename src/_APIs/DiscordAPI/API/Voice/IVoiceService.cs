// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAPI.API.Voice
{
    /// <summary>
    /// A service for Voice REST calls.
    /// </summary>
    public interface IVoiceService
    {
        /// <summary>
        /// Gets voice region list.
        /// </summary>
        /// <returns>List of available voice regions.</returns>
        [Get("/voice/regions")]
        Task<IEnumerable<VoiceRegion>> ListVoiceRegions();
    }
}
