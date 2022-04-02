// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Base;
using Discord.API.Models.Guilds.Interfaces;
using Discord.API.Models.Json.Users;
using Discord.API.Models.Managed.Base;
using System;

namespace Discord.API.Models.Managed.Users
{
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

        public ulong GuildId { get; set; }

        public ulong UserId { get; set; }

        public string Nickname { get; set; }

        public DateTimeOffset? JoinedAt { get; set; }

        public string? GuildAvatarId { get; set; }

        public ulong[] Roles { get; set; }

        public ulong? HoistedRole { get; set; }

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
    }
}
