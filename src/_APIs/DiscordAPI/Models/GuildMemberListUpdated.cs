using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class GuildMemberListUpdated
    {
        [JsonProperty("ops")]
        public IEnumerable<Operator> Operators { get; set; }

        [JsonProperty("online_count")]
        public int OnlineCount { get; set; }

        [JsonProperty("member_count")]
        public int MemberCount { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("groups")]
        public IEnumerable<Group> Groups { get; set; }
    }

}
