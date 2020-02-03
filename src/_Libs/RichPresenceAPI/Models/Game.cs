using Newtonsoft.Json;
using Quarrel.RichPresence.Models.Enums;

namespace Quarrel.RichPresence.Models
{
    /// <summary>
    /// Basic neccesities for a game display
    /// </summary>
    public class Game
    {
        public Game(string name, ActivityType type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Name of the game, usually what's displayed on one line
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Type of activity, determines the prefix
        /// </summary>
        [JsonProperty("type")]
        public ActivityType Type { get; set; }
    }
}
