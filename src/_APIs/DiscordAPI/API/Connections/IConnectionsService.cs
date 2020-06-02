// Copyright (c) Quarrel. All rights reserved.

using Refit;
using System.Threading.Tasks;

namespace DiscordAPI.API.Connections
{
    /// <summary>
    /// A service for connections REST calls.
    /// </summary>
    public interface IConnectionsService
    {
        /// <summary>
        /// Gets an Oauth url for a service via REST.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>A connection url.</returns>
        [Get("/v6/connections/{service}/authorize")]
        Task<Connection> GetOauthUrl([AliasAs("service")] string service);
    }
}
