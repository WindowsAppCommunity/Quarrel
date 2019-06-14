using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class GuildMemberRemove
    {
        [JsonProperty("guild_id")]
        public string guildId { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
