using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordAPI.API.Channel.Models
{
    public class MessageActivity
    {
        public string session_id { get; set; }
        public int type { get; set; }
        public string party_id { get; set; }
    }
    public class MessageUpsert
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("tts")]
        public bool TTS { get; set; }
        [JsonProperty("file")]
        public string file { get; set; }
      //  [JsonProperty("activity")]
      //  public MessageActivity Activity { get; set; }
    }

}
