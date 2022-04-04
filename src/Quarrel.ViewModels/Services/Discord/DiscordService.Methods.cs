// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using Quarrel.Models.Bindables;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
        public BindableGuild[] GetMyGuilds()
        {
            Guild[] myGuilds = _discordClient.GetMyGuilds();
            BindableGuild[] guilds = new BindableGuild[myGuilds.Length];
            for (int i = 0; i < myGuilds.Length; i++)
            {
                guilds[i] = new BindableGuild(myGuilds[i]);
            }

            return guilds;
        }
    }
}
