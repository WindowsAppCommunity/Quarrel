using Newtonsoft.Json;

namespace DiscordAPI.API.Guild.Models
{
    public class CreateGuildBan
    {
        [JsonProperty("delete-message-days")]
        public int DeleteMessageDays { get; set; }
    }
}
