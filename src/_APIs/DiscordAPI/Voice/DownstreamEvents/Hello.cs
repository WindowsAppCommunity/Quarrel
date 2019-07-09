using DiscordAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Voice.DownstreamEvents
{
    public class Hello
    {
        [JsonProperty("heartbeat_interval")]
        public int Heartbeatinterval { get; set; }
    }
}