using Newtonsoft.Json;

namespace GiphyAPI.Models
{
    public struct Meta
    {
        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("response_id")]
        public string ResponseId { get; set; }
    }
}
