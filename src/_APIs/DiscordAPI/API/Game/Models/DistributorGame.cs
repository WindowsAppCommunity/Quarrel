// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Game.Models
{
    /// <summary>
    /// A distributor game.
    /// </summary>
    public class DistributorGame
    {
        /// <summary>
        /// Gets or sets Distributor.
        /// </summary>
        [JsonProperty("distributor")]
        public string Distributor { get; set; }

        /// <summary>
        /// Gets or sets Sku.
        /// </summary>
        [JsonProperty("sku")]
        public string Sku { get; set; }
    }
}
