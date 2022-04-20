// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Quarrel.Services.Analytics
{
    public class AppCenterClientInfo
    {
        [JsonPropertyName("secret")]
        public string? Secret {get; set; }
    }
}
