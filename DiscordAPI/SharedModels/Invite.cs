using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct Invite
    {
        [JsonProperty("code")]
        public string String { get; set; }
        [JsonProperty("guild")]
        public InviteGuild Guild { get; set; }
        [JsonProperty("channel")]
        public InviteChannel Channel { get; set; }
    }

    public struct InviteGuild
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("splash_hash")]
        public string SplashHash { get; set; }
    }

    public struct InviteChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
