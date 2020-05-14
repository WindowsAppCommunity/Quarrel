using DiscordAPI.Models;
using DiscordAPI.Models.Messages;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.API.Guild.Models
{
    public class SearchResults
    {
        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
        [JsonProperty("analytics_id")]
        public string AnalyticsId { get; set; }
        [JsonProperty("messages")]
        public IEnumerable<IEnumerable<Message>> Messages { get; set; }
    }
}
