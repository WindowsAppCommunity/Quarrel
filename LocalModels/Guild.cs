using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord_UWP.SharedModels;

namespace Discord_UWP.LocalModels
{
    public class Guild
    {
        public Guild()
        {
            Raw = new SharedModels.Guild();
            Position = 0;
        }

        public Guild(SharedModels.Guild guild)
        {
            Raw = guild;
            Position = 0;
        }

        public Role GetHighestRole(IEnumerable<string> inputRoles)
        {
            Role returnRole = new Role() { Position = 1000 }; //HACK: this could be better
            foreach (var role in roles.Values)
            {
                if (role.Position < returnRole.Position)
                {
                    returnRole = role;
                }
            }
            return returnRole;
        }
        public string GetHighestHoistRoleId(IEnumerable<string> inputRoles)
        {
            Role returnRole = new Role() { Position = 1000 }; //HACK: this could be better
            var rolelist = inputRoles.OrderBy(x => roles[x].Position).Reverse().ToList();
            for (int i = 0; i < rolelist.Count(); i++)
            {
                if (roles[rolelist[i]].Hoist)
                {
                    return rolelist[i];
                }
            }
            return null;
        }

        public Permissions permissions
        {
            get { return new Permissions(Raw.Id); }
        }

        public SharedModels.Guild Raw { get; set; }
        public ConcurrentDictionary<string, GuildMember> members = new ConcurrentDictionary<string, GuildMember>();
        public ConcurrentDictionary<string, GuildChannel> channels = new ConcurrentDictionary<string, GuildChannel>();
        public ConcurrentDictionary<string, Role> roles = new ConcurrentDictionary<string, Role>();
        public bool valid = true;

        public int Position { get; set; }
    }
}
