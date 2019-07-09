using DiscordAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Voice.DownstreamEvents
{
    public class VoiceData
    {
        public float[] data { get; set; }
        public uint samples { get; set; }
    }
}