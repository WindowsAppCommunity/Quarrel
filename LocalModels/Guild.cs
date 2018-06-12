using System;
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
            Role returnRole = new Role() { Position = 1000 }; //TODO: this could be better
            foreach (var role in roles.Values)
            {
                if (role.Position < returnRole.Position)
                {
                    returnRole = role;
                }
            }
            return returnRole;
        }

        public Permissions permissions
        {
            get { return new Permissions(Raw.Id); }
        }

        public SharedModels.Guild Raw { get; set; }
        public Dictionary<string, GuildMember> members = new Dictionary<string, GuildMember>();
        public Dictionary<string, GuildChannel> channels = new Dictionary<string, GuildChannel>();
        public Dictionary<string, Role> roles = new Dictionary<string, Role>();
        public bool valid = true;

        public int Position { get; set; }
    }
}
