﻿// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Base;
using Discord.API.Models.Enums.Users;
using Discord.API.Models.Json.Users;
using Discord.API.Models.Users.Interfaces;
using System;

namespace Discord.API.Models.Users
{
    /// <summary>
    /// A guild member managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public class GuildMember : DiscordItem, IGuildMember
    {
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
                switch (Presence.Status)
                {
                    case UserStatus.Online:
                        jsonPresence.Status = "online";
                        break;
                    case UserStatus.Idle:
                        jsonPresence.Status = "idle";
                        break;
                    case UserStatus.AFK:
                        jsonPresence.Status = "afk";
                        break;
                    case UserStatus.DoNotDisturb:
                        jsonPresence.Status = "dnd";
                        break;
                    default:
                    case UserStatus.Offline:
                        jsonPresence.Status = "offline";
                        break;
                }
            }

            return jsonPresence;
        }
    }
}
