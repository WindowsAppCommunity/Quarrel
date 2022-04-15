// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Base;
using Discord.API.Models.Enums.Users;
using Discord.API.Models.Guilds;
using Discord.API.Models.Json.Users;
using Discord.API.Models.Roles;
using Discord.API.Models.Users.Interfaces;
using System;
using System.Collections.Generic;

namespace Discord.API.Models.Users
{
    /// <summary>
    /// A guild member managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public class GuildMember : DiscordItem, IGuildMember
    {
        private readonly HashSet<ulong> _roles;

        internal GuildMember(JsonGuildMember jsonMember, DiscordClient context)
            : this(jsonMember, jsonMember.GuildId, context)
        {
        }

        internal GuildMember(JsonGuildMember jsonMember, ulong? guildId, DiscordClient context) :
            base(context)
        {
            Guard.IsNotNull(guildId, nameof(guildId));

            GuildId = guildId.Value;

            UserId = jsonMember.User.Id;
            JoinedAt = jsonMember.JoinedAt;
            Nickname = jsonMember.Nickname;
            GuildAvatarId = jsonMember.Avatar;
            Roles = jsonMember.Roles;
            HoistedRole = jsonMember.HoistedRole;

            _roles = new HashSet<ulong>();
            foreach (var role in Roles)
            {
                _roles.Add(role);
            }

            _roles.Add(GuildId);
        }

        /// <inheritdoc/>
        public ulong GuildId { get; protected set; }

        /// <inheritdoc/>
        public ulong UserId { get; protected set; }

        /// <inheritdoc/>
        public string Nickname { get; protected set; }

        /// <inheritdoc/>
        public DateTimeOffset? JoinedAt { get; protected set; }

        /// <inheritdoc/>
        public string? GuildAvatarId { get; protected set; }

        /// <inheritdoc/>
        public ulong[] Roles { get; protected set; }

        /// <inheritdoc/>
        public ulong? HoistedRole { get; protected set; }

        /// <inheritdoc/>
        public Presence? Presence { get; internal set; }

        /// <summary>
        /// Get a value indicating whether or not the user has a role.
        /// </summary>
        /// <param name="roleId">The id of the role to check for the user.</param>
        /// <returns>True if the user has the role, false otherwise.</returns>
        public bool HasRole(ulong roleId)
        {
            return _roles.Contains(roleId);
        }

        /// <summary>
        /// Gets an the roles the user has.
        /// </summary>
        /// <returns>An array of roles that the user has.</returns>
        public Role[] GetRoles()
        {
            Guild? guild = Context.GetGuildInternal(GuildId);
            Guard.IsNotNull(guild, nameof(guild));

            Role[] roles = new Role[Roles.Length+1];
            for (int i = 0; i < Roles.Length; i++)
            {
                Role? role = guild.GetRole(Roles[i]);
                Guard.IsNotNull(role, nameof(role));
                roles[i+1] = role;
            }
            
            Role? everyoneRole = guild.GetRole(guild.Id);
            Guard.IsNotNull(everyoneRole, nameof(everyoneRole));
            roles[0] = everyoneRole;

            return roles;
        }

        internal void UpdateFromJsonGuildMember(JsonGuildMember jsonGuildMember)
        {
            Guard.IsEqualTo(UserId, jsonGuildMember.User.Id, nameof(UserId));
            Guard.IsEqualTo(GuildId, jsonGuildMember.User.Id, nameof(GuildId));

            Nickname = jsonGuildMember.Nickname ?? Nickname;
            JoinedAt = jsonGuildMember.JoinedAt ?? JoinedAt;
            GuildAvatarId = jsonGuildMember.Avatar ?? GuildAvatarId;
            Roles = jsonGuildMember.Roles ?? Roles;
            HoistedRole = jsonGuildMember.HoistedRole ?? HoistedRole;
        }

        internal JsonPresence? ToJsonPresence()
        {
            User? user = Context.GetUserInternal(UserId);

            if (user is null)
            {
                return null;
            }

            var jsonPresence = new JsonPresence()
            {
                GuildId = GuildId,
                Roles = Roles,
                User = user.ToRestUser(),
            };

            if (Presence is not null)
            {
                jsonPresence.Status = Presence.Status switch
                {
                    UserStatus.Online => "online",
                    UserStatus.Idle => "idle",
                    UserStatus.AFK => "afk",
                    UserStatus.DoNotDisturb => "dnd",
                    UserStatus.Invisible => "invisible",
                    _ => "offline",
                };
            }

            return jsonPresence;
        }
    }
}
