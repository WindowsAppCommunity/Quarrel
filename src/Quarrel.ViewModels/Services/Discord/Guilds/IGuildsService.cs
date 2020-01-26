using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables;

namespace Quarrel.ViewModels.Services.Discord.Guilds
{
    public interface IGuildsService
    {
        ConcurrentDictionary<string, GuildSetting> GuildSettings { get; }
        ConcurrentDictionary<string, BindableGuildMember> AllMembers { get; }
        IDictionary<string, BindableGuild> AllGuilds { get; }
        string CurrentGuildId { get; }
        BindableGuild CurrentGuild { get; }
        BindableGuildMember GetGuildMember(string memberId, string guildId);
        IReadOnlyDictionary<string, GuildMember> GetAndRequestGuildMembers(IEnumerable<string> memberIds, string guildId);
    }
}
