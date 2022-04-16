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
            switch (jsonPresence.Status)
            {
                case "online":
                    Status = UserStatus.Online;
                    break;
                case "idle":
                    Status = UserStatus.Idle;
                    break;
                case "dnd":
                    Status = UserStatus.DoNotDisturb;
                    break;
                case "offline":
                    Status = UserStatus.Offline;
                    break;
                default:
                    Status = UserStatus.Offline;
                    break;
            }
        }

        /// <summary>
        /// Gets the online status.
        /// </summary>
        public UserStatus Status { get; set; }
    }
}
