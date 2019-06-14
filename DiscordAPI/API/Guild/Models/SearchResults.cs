using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

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
