using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct GuildChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("is_private")]
        public bool Private { get; set; }
        [JsonProperty("permission_overwrites")]
        public IEnumerable<Overwrite> PermissionOverwrites { get; set; }
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public string UserLimit { get; set; }
    }
}
