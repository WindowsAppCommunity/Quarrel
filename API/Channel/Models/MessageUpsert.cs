using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Discord_UWP.API.Channel.Models
{
    public struct MessageActivity
    {
        public string session_id { get; set; }
        public int type { get; set; }
        public string party_id { get; set; }
    }
    public struct MessageUpsert
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
