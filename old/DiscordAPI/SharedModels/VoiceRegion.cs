using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class VoiceRegion
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("sample_hostname")]
        public string SampleHostname { get; set; }
        [JsonProperty("sample_port")]
        public int SamplePort { get; set; }
        [JsonProperty("vip")]
        public bool Vip { get; set; }
        [JsonProperty("optimal")]
        public bool Optimal { get; set; }
    }
}
