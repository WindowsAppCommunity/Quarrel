// Quarrel © 2022

using Discord.API.Models.Enums.Users;
using Discord.API.Models.Json.Users;
using Quarrel.Client.Models.Users.Interfaces;

namespace Quarrel.Client.Models.Users
{
    /// <summary>
    /// A user presence managed by a <see cref="QuarrelClient"/>
    /// </summary>
    public class Presence : IPresence
    {
        internal Presence(JsonPresence jsonPresence)
        {
            Status = jsonPresence.Status switch
            {
                "online" => UserStatus.Online,
                "idle" => UserStatus.Idle,
                "dnd" => UserStatus.DoNotDisturb,
                "offline" => UserStatus.Offline,
                _ => UserStatus.Offline,
            };
        }

        /// <summary>
        /// Gets the online status.
        /// </summary>
        public UserStatus Status { get; set; }
    }
}
