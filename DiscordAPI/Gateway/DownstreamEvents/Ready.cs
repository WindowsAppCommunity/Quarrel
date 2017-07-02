using Discord_UWP.SharedModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Gateway.DownstreamEvents
{
    public struct Ready
    {
        [JsonProperty("v")]
        public int GatewayVersion { get; set; }

        [JsonProperty("user_settings")]
        public UserSettings Settings { get; set; }

        [JsonProperty("user_guild_settings")]
        public IEnumerable<GuildSetting> GuildSettings { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("private_channels")]
        public IEnumerable<DirectMessageChannel> PrivateChannels { get; set; }

        [JsonProperty("guilds")]
        public IEnumerable<Guild> Guilds { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("presences")]
        public IEnumerable<Presence> Presences { get; set; }

        [JsonProperty("relationships")]
        public IEnumerable<Friend> Friends { get; set; }

        [JsonProperty("read_state")]
        public IEnumerable<ReadState> ReadStates { get; set; }

        [JsonProperty("notes")]
        public Dictionary<string,string> Notes { get; set; }

        [JsonProperty("friend_suggestion_count")]
        public int FriendSuggestionCount { get; set; }

        [JsonProperty("connected_accounts")]
        public IEnumerable<Connection> ConnectedAccount { get; set; }

        [JsonProperty("_trace")]
        public IEnumerable<string> Trace { get; set; }
    }
}
