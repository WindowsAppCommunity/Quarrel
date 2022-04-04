// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Guilds;

namespace Discord.API
{
    public partial class DiscordClient
    {
        public Guild[] GetMyGuilds()
        {
            ulong[] order = _settings.GuildOrder;
            Guild[] guildArray = new Guild[order.Length];

            for (int i = 0; i < order.Length; i++)
            {
                Guild? guild = GetGuildInternal(order[i]);
                if (guild != null)
                {
                    guildArray[i] = guild;
                }
            }

            return guildArray;
        }
    }
}
