// Quarrel © 2022

using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Settings
{
    internal record JsonMuteConfig
    {
        [JsonPropertyName("selected_time_window")]
        public int SelectedTimeWindow { get; set; }

        [JsonPropertyName("end_time")]
        public DateTime EndTime { get; set; }
    }
}
