// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using System;

namespace Discord.API
{
    /// <inheritdoc/>
    public partial class DiscordClient
    {
        /// <summary>
        /// Gets the user's guild according to their order in settings.
        /// </summary>
        public Guild[] GetMyGuilds()
        {
            ulong[] order = _settings.GuildOrder;
            Guild[] guildArray = new Guild[order.Length];

            int realCount = 0;
            for (int i = 0; i < order.Length; i++)
            {
                Guild? guild = GetGuildInternal(order[realCount]);
                if (guild != null)
                {
                    guildArray[i] = guild;
                    realCount++;
                }
            }

            Array.Resize(ref guildArray, realCount);

            return guildArray;
        }
    }
}
