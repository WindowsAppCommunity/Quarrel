// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using Discord.API.Models.Users;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Guilds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public interface IDiscordService
    {
        SelfUser GetMe();

        Task LoginAsync(string token);

        BindableGuild[] GetMyGuilds();

        BindableChannel[] GetGuildChannels(Guild guild);

        IEnumerable<BindableChannelGroup>? GetGuildChannelsGrouped(Guild guild);
    }
}
