using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class GuildChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("parent_id")]
        public string ParentId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 0 = Text,
        /// 1 = DM,
        /// 2 = Voice,
        /// 3 = Group DM,
        /// 4 = Category
        /// </summary>
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
        [JsonProperty("nsfw")]
        public bool NSFW { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }

        public void UpdateLMID(string id)
        {
            LastMessageId = id;
        }
    }
}
