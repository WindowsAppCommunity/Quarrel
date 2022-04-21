// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Quarrel.RichPresence.Models
{
    /// <summary>
    /// The base class of an acitivity to display with rich presence.
    /// </summary>
    public class Activity
    {
        [JsonPropertyName("name")]
        public string Name { get; }
    }
}
