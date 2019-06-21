using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace Quarrel.LocalModels
{
    public class Guild
    {
        /// <summary>
        /// Create blank guild object
        /// </summary>
        public Guild()
        {
            Raw = new DiscordAPI.SharedModels.Guild();
            Position = 0;
        }

        /// <summary>
        /// Create LocalModel based on API model
        /// </summary>
        /// <param name="guild">API model</param>
        public Guild(DiscordAPI.SharedModels.Guild guild)
        {
            Raw = guild;
            Position = 0;
        }

        /// <summary>
        /// Determine which role has the highest position
        /// </summary>
        /// <param name="inputRoles">Roles to check</param>
        /// <returns>Role Object of highest ID</returns>
        public Role GetHighestRole(IEnumerable<string> inputRoles)
        {
            Role returnRole = new Role() { Position = 1000 }; //HACK: this could be better

            // Check each role
            foreach (var role in roles.Values)
            {
                // If the role is lower (lower is higher)
                if (role.Position < returnRole.Position)
                {
                    // Mark new role as highest role
                    returnRole = role;
                }
            }
            return returnRole;
        }

        /// <summary>
        /// Determine which role has the highest position
        /// </summary>
        /// <param name="inputRoles">Roles to check</param>
        /// <returns>Id of highest role</returns>
        public string GetHighestHoistRoleId(IEnumerable<string> inputRoles)
        {
            Role returnRole = new Role() { Position = 1000 }; //HACK: this could be better

            // Sort by position
            var rolelist = inputRoles.OrderBy(x => roles[x].Position).Reverse().ToList();
            for (int i = 0; i < rolelist.Count(); i++)
            {
                // Make sure it has Hoist
                if (roles[rolelist[i]].Hoist)
                {
                    return rolelist[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Get permissions for Guild
        /// </summary>
        public Permissions permissions
        {
            get { return new Permissions(Raw.Id); }
        }

        /// <summary>
        /// API Guild object
        /// </summary>
        public DiscordAPI.SharedModels.Guild Raw { get; set; }

        /// <summary>
        /// Member objects paired to Ids
        /// </summary>
        public ConcurrentDictionary<string, GuildMember> members = new ConcurrentDictionary<string, GuildMember>();

        /// <summary>
        /// Channel objects paired to Ids
        /// </summary>
        public ConcurrentDictionary<string, GuildChannel> channels = new ConcurrentDictionary<string, GuildChannel>();

        /// <summary>
        /// Role objects paired to Ids
        /// </summary>
        public ConcurrentDictionary<string, Role> roles = new ConcurrentDictionary<string, Role>();

        /// <summary>
        /// False when Server is down
        /// </summary>
        public bool valid = true;

        /// <summary>
        /// Position in Server list
        /// </summary>
        public int Position { get; set; }
    }
}
