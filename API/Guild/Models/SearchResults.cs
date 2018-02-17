using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord_UWP.SharedModels;

namespace Discord_UWP.API.Guild.Models
{
    public struct SearchResults
    {
        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
        [JsonProperty("analytics_id")]
        public string AnalyticsId { get; set; }
        [JsonProperty("messages")]
        public IEnumerable<IEnumerable<Message>> Messages { get; set; }
    }
}
