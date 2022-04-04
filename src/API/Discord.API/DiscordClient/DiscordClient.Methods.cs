// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using System;

namespace Discord.API
{
    public partial class DiscordClient
    {
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
