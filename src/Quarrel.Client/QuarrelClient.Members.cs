// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Users;
using Quarrel.Client.Models.Users;
using System.Collections.Concurrent;

namespace Quarrel.Client
{
    public partial class QuarrelClient
    {
        /// <summary>
        /// A class managing the <see cref="QuarrelClient"/>'s members.
        /// </summary>
        public class QuarrelClientMembers
        {
            private readonly QuarrelClient _client;
            private readonly ConcurrentDictionary<(ulong GuildId, ulong UserId), GuildMember> _guildsMemberMap;

            /// <summary>
            /// Initializes a new instance of the <see cref="QuarrelClientMembers"/> class.
            /// </summary>
            internal QuarrelClientMembers(QuarrelClient client)
            {
                _client = client;

                _guildsMemberMap = new();
            }

            /// <summary>
            /// Gets a guild member by guild and user id.
            /// </summary>
            /// <param name="guildId">The id for the guild of the guild member.</param>
            /// <param name="userId">The id for the user in the guild.</param>
            /// <returns></returns>
            public GuildMember? GetGuildMember(ulong guildId, ulong userId)
            {
                if (_guildsMemberMap.TryGetValue((guildId, userId), out var member))
                {
                    return member;
                }

                return null;
            }

            /// <summary>
            /// Gets the current user as a guild member in a specific guild.
            /// </summary>
            /// <param name="guildId">The id of the guild to get the guild member for.</param>
            public GuildMember? GetMyGuildMember(ulong guildId)
            {
                Guard.IsNotNull(_client.Self.CurrentUser, nameof(Self.CurrentUser));
                return GetGuildMember(guildId, _client.Self.CurrentUser.Id);
            }

            internal bool AddGuildMember(ulong guildId, JsonGuildMember jsonGuildMember)
            {
                var member = new GuildMember(jsonGuildMember, guildId, _client);
                if (_guildsMemberMap.TryAdd((guildId, member.UserId), member))
                {
                    _client.Users.AddUser(jsonGuildMember.User);

                    return true;
                }
                return false;
            }

            internal bool UpdateGuildMember(ulong guildId, JsonGuildMember jsonGuildMember)
            {
                if (!_guildsMemberMap.TryGetValue((guildId, jsonGuildMember.User.Id), out GuildMember member)) return false;

                member.UpdateFromJsonGuildMember(jsonGuildMember);
                return true;

            }

            internal bool RemoveGuildMember(ulong guildId, JsonGuildMember jsonGuildMember)
            {
                return _guildsMemberMap.TryRemove((guildId, jsonGuildMember.User.Id), out _);
            }
        }
    }
}
