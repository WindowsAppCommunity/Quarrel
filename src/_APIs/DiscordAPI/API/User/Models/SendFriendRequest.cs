using Newtonsoft.Json;

namespace DiscordAPI.API.User.Models
{
    public class SendFriendRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("discriminator")]
        public int Discriminator { get; set; }
    }
}
