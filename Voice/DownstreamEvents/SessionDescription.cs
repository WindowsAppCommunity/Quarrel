using Discord_UWP.SharedModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice.DownstreamEvents
{
    public class SessionDescription
    {
        [JsonProperty("secret_key")]
        public byte[] SecretKey { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
}