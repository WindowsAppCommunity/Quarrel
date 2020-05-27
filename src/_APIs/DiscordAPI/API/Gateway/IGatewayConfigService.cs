// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Refit;
using System.Threading.Tasks;

namespace DiscordAPI.API.Gateway
{
    /// <summary>
    /// A service for Gateway configuration REST calls.
    /// </summary>
    public interface IGatewayConfigService
    {
        /// <summary>
        /// Get gateway config data via REST.
        /// </summary>
        /// <returns>Gateway configuration info.</returns>
        [Get("/gateway")]
        Task<GatewayConfig> GetGatewayConfig();
    }
}
