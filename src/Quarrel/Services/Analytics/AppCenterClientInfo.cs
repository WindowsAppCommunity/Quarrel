// Special thanks to Sergio Pedri for the basis of this design
// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace Quarrel.Services.Analytics
{
    /// <summary>
    /// A simple model that maps the JSON file that stores the secret token for an AppCenter app.
    /// </summary>
    public sealed class AppCenterClientInfo
    {
        /// <summary>
        /// Gets or sets the secret for the app center service.
        /// </summary>
        [JsonProperty("secret")]
        public string Secret { get; set; }
    }
}
