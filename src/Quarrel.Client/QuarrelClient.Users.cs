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
        /// A class managing the <see cref="QuarrelClient"/>'s users.
        /// </summary>
        public class QuarrelClientUsers
        {
            private readonly QuarrelClient _client;
            private readonly ConcurrentDictionary<ulong, User> _userMap;

            /// <summary>
            /// Initializes a new instance of the <see cref="QuarrelClientUsers"/> class.
            /// </summary>
            internal QuarrelClientUsers(QuarrelClient client)
            {
                _client = client;

                _userMap = new ConcurrentDictionary<ulong, User>();
            }

            /// <summary>
            /// Gets a user by id.
            /// </summary>
            /// <param name="id">The id of the user to get.</param>
            public User? GetUser(ulong id)
            {
                _userMap.TryGetValue(id, out var user);
                return user;
            }

            internal User GetOrAddUser(JsonUser jsonUser)
            {
                if (_userMap.TryGetValue(jsonUser.Id, out User? user)) return user;

                AddUser(jsonUser);
                user = GetUser(jsonUser.Id);
                Guard.IsNotNull(user, nameof(user));
                return user;
            }

            internal bool AddUser(JsonUser jsonUser)
            {
                var user = new User(jsonUser, _client);
                return AddUser(user);
            }

            internal bool AddUser(User user)
            {
                return _userMap.TryAdd(user.Id, user);
            }

            internal bool UpdateUser(JsonUser jsonUser)
            {
                if (_userMap.TryGetValue(jsonUser.Id, out User user))
                {
                    user.UpdateFromRestUser(jsonUser);
                    return true;
                }

                return false;
            }

            internal bool RemoveUser(ulong userId)
            {
                return _userMap.TryRemove(userId, out _);
            }

            internal bool AddPresence(JsonPresence jsonPresence)
            {
                Guard.IsNotNull(jsonPresence.User, nameof(jsonPresence.User));

                if (GetUser(jsonPresence.User.Id) is null)
                {
                    AddUser(jsonPresence.User);
                }

                if (!_userMap.TryGetValue(jsonPresence.User.Id, out User user)) return false;

                user.Presence = new Presence(jsonPresence);
                return true;

            }

            internal bool AddRelationship(JsonRelationship jsonRelationship)
            {
                Guard.IsNotNull(jsonRelationship.User, nameof(jsonRelationship.User));

                User user = GetOrAddUser(jsonRelationship.User);

                bool status = true;
                user.RelationshipType = jsonRelationship.Type;

                if (jsonRelationship.Presence is not null)
                {
                    bool added = AddPresence(jsonRelationship.Presence);
                    status = status && added;
                }

                return status;
            }
        }
    }
}
