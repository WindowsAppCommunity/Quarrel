using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;
using Newtonsoft.Json;

namespace DiscordAPI.SharedModels
{
    public class Webhook
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public class ModifyWebhook
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }
    public class ModifyWebhookAvatar : ModifyWebhook
    {
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }
}
