using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.Channel.Models
{
    public class ModifyChannel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public int UserLimit { get; set; }
        [JsonProperty("nsfw")]
        public bool NSFW { get; set; }
        [JsonProperty("permission_overwrites")]
        public IEnumerable<Overwrite> PermissionOverwrites { get; set; }
        [JsonProperty("parent_id")]
        public string ParentId { get; set; }
    }
}
