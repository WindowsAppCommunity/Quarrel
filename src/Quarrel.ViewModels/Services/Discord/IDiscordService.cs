// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Guilds;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public interface IDiscordService
    {
        Task LoginAsync(string token);

        BindableGuild[] GetMyGuilds();

        BindableChannel[] GetChannels(Guild guild);
    }
}
