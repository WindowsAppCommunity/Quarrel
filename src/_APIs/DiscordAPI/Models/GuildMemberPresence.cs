using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class GuildMemberPresence : GuildMember
    {
        [JsonProperty("presence")]
        public Presence Presence { get; set; }
    }

}
