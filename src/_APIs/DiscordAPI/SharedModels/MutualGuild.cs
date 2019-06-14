using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace DiscordAPI.SharedModels
{
    public class MutualGuild
    {
        [JsonProperty("nick")]
        public string Nick { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public Visibility NickVisibility { get; set; }
    }
}
