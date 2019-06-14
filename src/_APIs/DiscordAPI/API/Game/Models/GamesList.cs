using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordAPI.API.Game
{

    public class Executable
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("os")]
        public string Os { get; set; }
    }

    public class DistributorGame
    {
        [JsonProperty("distributor")]
        public string Distributor { get; set; }
        [JsonProperty("sku")]
        public string Sku { get; set; }
    }

    public class GameListItem
    {
        [JsonProperty("executables")]
        public List<Executable> Executables { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("splash")]
        public string Splash { get; set; }
        [JsonProperty("aliases")]
        public List<string> Aliases { get; set; }
        [JsonProperty("distributor_applications")]
        public List<DistributorGame> DistributorGames { get; set; }
        [JsonProperty("developers")]
        public List<string> Developers { get; set; }
        [JsonProperty("publishers")]
        public List<string> Publishers { get; set; }
        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }
        [JsonProperty("youtube_trailer_video_id")]
        public string YoutubeTrailerId { get; set; }
    }
}
