using Newtonsoft.Json;

namespace DiscordAPI.API.User.Models
{
    public class SendFriendRequestResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
