using Newtonsoft.Json;

namespace DiscordAPI.API.User.Models
{
    public class Note
    {
        [JsonProperty("note")]
        public string Content { get; set; }
    }
}
