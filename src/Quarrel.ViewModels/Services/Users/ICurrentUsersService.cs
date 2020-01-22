using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.Services.Users
{
    public interface ICurrentUsersService
    {
        ConcurrentDictionary<string, BindableGuildMember> Users { get; }
        ConcurrentDictionary<string, BindableGuildMember> DMUsers { get; }
        ConcurrentDictionary<string, BindableFriend> Friends { get; }
        ConcurrentDictionary<string, GuildSetting> GuildSettings { get; }
        ConcurrentDictionary<string, ChannelOverride> ChannelSettings { get; }

        BindableUser CurrentUser { get; }

        /// <summary>
        /// Imporant, do not bind to
        /// </summary>
        BindableGuildMember CurrentGuildMember { get; }

        UserSettings CurrentUserSettings { get; }


        /// <summary>
        /// This will return the guild members stored localy
        /// If any members are not stored localy they will be requested
        /// </summary>
        /// <param name="memberIds"></param>
        /// <param name="guildId"></param>
        /// <returns></returns>
        IReadOnlyDictionary<string, GuildMember> GetAndRequestGuildMembers(IEnumerable<string> memberIds, string guildId);
        BindableGuildMember GetGuildMember(string memberId, string guildId);
        Presence GetUserPrecense(string userId);
        void UpdateUserPrecense(string userId, Presence presence);
    }
}
