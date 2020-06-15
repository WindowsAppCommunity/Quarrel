// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Models.Bindables.Users;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Discord.Guilds
{
    /// <summary>
    /// Manages all guild information.
    /// </summary>
    public interface IGuildsService
    {
        IDictionary<string, ConcurrentDictionary<string, BindableGuildMember>> GuildUsers { get; }

        /// <summary>
        /// Gets the current guild.
        /// </summary>
        BindableGuild CurrentGuild { get; }

        /// <summary>
        /// Gets a guild by Id.
        /// </summary>
        /// <param name="guildId">The guild's id.</param>
        /// <returns>The requested <see cref="BindableGuild"/>.</returns>
        BindableGuild GetGuild(string guildId);

        /// <summary>
        /// Adds or updates a guild in the guild list.
        /// </summary>
        /// <param name="guildId">The id of the guild.</param>
        /// <param name="guild">The new guild value.</param>
        void AddOrUpdateGuild(string guildId, BindableGuild guild);

        /// <summary>
        /// Removes a guild from the guild list.
        /// </summary>
        /// <param name="guildId">The guild's id.</param>
        void RemoveGuild(string guildId);

        /// <summary>
        /// Gets the settings for a guild.
        /// </summary>
        /// <param name="guildId">The guild's id.</param>
        /// <returns>The requested <see cref="GuildSetting"/>s.</returns>
        GuildSetting GetGuildSetting(string guildId);

        /// <summary>
        /// Adds or updates an item in the guilds settings list.
        /// </summary>
        /// <param name="guildId">The guild's id.</param>
        /// <param name="guildSetting">The guild's settings.</param>
        void AddOrUpdateGuildSettings(string guildId, GuildSetting guildSetting);

        /// <summary>
        /// Gets a guild member by user id and guild id.
        /// </summary>
        /// <param name="memberId">The GuildMember user id.</param>
        /// <param name="guildId">The GuildMember guild id.</param>
        /// <returns>The requested <see cref="BindableGuildMember"/> or null.</returns>
        BindableGuildMember GetGuildMember(string memberId, string guildId);

        /// <summary>
        /// Gets a guild member by username, discriminator and guild id.
        /// </summary>
        /// <param name="username">The GuildMember username.</param>
        /// <param name="discriminator">The GuildMember discriminator.</param>
        /// <param name="guildId">The GuildMember guild id.</param>
        /// <returns>The requested <see cref="BindableGuildMember"/> or null.</returns>
        BindableGuildMember GetGuildMember(string username, string discriminator, string guildId);

        /// <summary>
        /// Gets a collection of GuildMembers by user ids and guild id.
        /// </summary>
        /// <param name="memberIds">The member ids.</param>
        /// <param name="guildId">The guild id.</param>
        /// <returns>A hashed collection of guild members by user id.</returns>
        IReadOnlyDictionary<string, BindableGuildMember> GetAndRequestGuildMembers(IEnumerable<string> memberIds, string guildId);

        /// <summary>
        /// Quries a guild's members by <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The query to filter by.</param>
        /// <param name="guildId">The guild to query.</param>
        /// <param name="take">The max number of members to return.</param>
        /// <returns>The top matches for members by the <paramref name="query"/> in <paramref name="guildId"/>.</returns>
        IEnumerable<BindableGuildMember> QueryGuildMembers(string query, string guildId, int take= 10);
    }
}
