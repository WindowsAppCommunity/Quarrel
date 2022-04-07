// Adam Dernis © 2022

using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Guilds;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Guilds;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
        public BindableGuild[] GetMyGuilds()
        {
            Guild[] rawGuilds = _discordClient.GetMyGuilds();
            BindableGuild[] guilds = new BindableGuild[rawGuilds.Length];
            for (int i = 0; i < rawGuilds.Length; i++)
            {
                guilds[i] = new BindableGuild(rawGuilds[i]);
            }

            return guilds;
        }

        public BindableChannel?[] GetChannels(Guild guild)
        {
            Channel[] rawChannels = guild.GetChannels();
            BindableChannel?[] channels = new BindableChannel[rawChannels.Length];
            for (int i = 0; i < rawChannels.Length; i++)
            {
                channels[i] = BindableChannel.Create(rawChannels[i]);
            }

            return channels;
        }
    }
}
