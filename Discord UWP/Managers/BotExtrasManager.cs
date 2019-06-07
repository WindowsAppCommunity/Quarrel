using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Managers
{
    public static class BotExtrasManager
    {
        public class SpecialFeature
        {
            /// <summary>
            /// Create a new special feature for a bot
            /// </summary>
            /// <param name="name">The name of the special feature (like "View statistics")</param>
            /// <param name="icon">The Segoe MDL2 Glyph for the feature</param>
            /// <param name="url">The URL to reach</param>
            public SpecialFeature(string name, string icon, string url)
            {
                Name = name;
                Url = url;
                Icon = icon;
            }
            public string Url { get; set; }
            public string Icon { get; set; }
            public string Name { get; set; }
        }
        
        /// <summary>
        /// Get bot features by id
        /// </summary>
        /// <param name="botId">Id of bot</param>
        /// <returns>List of features</returns>
        public static SpecialFeature[] GetBotFeatures(string botId)
        {
            var lastfeature = new SpecialFeature("DiscordBots.org", "", "https://discordbots.org/bot/" + botId);
            switch (botId)
            {
                //CARBON
                case "109338686889476096":
                    {
                        return new SpecialFeature[]{
                            new SpecialFeature("Server statistics", "", "https://www.carbonitex.net/discord/server?s=" + App.CurrentGuildId),
                            lastfeature
                        };
                    }
                //YAGPDB
                case "204255221017214977":
                    {
                        return new SpecialFeature[]{
                            new SpecialFeature("Server statistics", "", "https://yagpdb.xyz/public/" + App.CurrentGuildId + "/stats"),
                            lastfeature
                        };
                    }
                //MEE6
                case "159985870458322944":
                    {//
                        return new SpecialFeature[]{
                            new SpecialFeature("Leaderboard", "", "https://mee6.xyz/leaderboard/" + App.CurrentGuildId),
                            lastfeature
                        };
                    }
                //UNBELIEVABOT
                case "292953664492929025":
                {
                    return new SpecialFeature[]{
                        new SpecialFeature("Leaderboard", "", "https://unbelievable.pizza/leaderboard/" + App.CurrentGuildId),
                        lastfeature
                    };
                    }
            }
            return new SpecialFeature[] { lastfeature };
        }
    }
}
