// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Messages
{
    internal record JsonComponent
    {
        [JsonPropertyName("type")]
        public ComponentType Type { get; set; }
    }
}
