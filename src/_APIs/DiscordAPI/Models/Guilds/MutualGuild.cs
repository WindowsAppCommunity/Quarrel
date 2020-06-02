// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.Models.Guilds
{
    public class MutualGuild
    {
        [JsonProperty("nick")]
        public string Nick { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public bool HasNickname => !string.IsNullOrEmpty(Nick);
    }
}
