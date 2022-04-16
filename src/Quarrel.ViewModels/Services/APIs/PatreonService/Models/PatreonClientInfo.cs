// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Quarrel.Services.APIs.PatreonService.Models
{
    /// <summary>
    /// A json model for the patreon client information.
    /// </summary>
    public class PatreonClientInfo
    {
        /// <summary>
        /// Gets the client id.
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        
        /// <summary>
        /// Gets the client secret.
        /// </summary>
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }
        
        /// <summary>
        /// Gets the client access token.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Gets the client refresh token.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
