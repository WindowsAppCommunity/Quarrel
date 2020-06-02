// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models.Guilds
{
    public class GuildSync
    {
        [JsonProperty("id")]
        public string GuildId { get; set; }
        [JsonProperty("large")]
        public bool IsLarge { get; set; }
        [JsonProperty("members")]
        public IEnumerable<GuildMember> Members { get; set; }
        [JsonProperty("presences")]
        public IEnumerable<Presence> Presences { get; set; }

    }
}
