using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord_UWP.SharedModels;

namespace Discord_UWP.LocalModels
{
    public class GuildChannel
    {
        public GuildChannel(SharedModels.GuildChannel channel)
        {
            raw = channel;
            GetPermissions();
        }

        public GuildChannel(SharedModels.GuildChannel channel, string guildId)
        {
            raw = channel;
            raw.GuildId = guildId;
            GetPermissions();
        }

        void GetPermissions()
        {
            permissions = LocalState.Guilds[raw.GuildId].permissions;
            foreach (Overwrite overwrite in raw.PermissionOverwrites.TakeWhile(x => x.Type == "role" ? LocalState.Guilds[raw.GuildId].members[LocalState.CurrentUser.Id].Roles.Contains(x.Id) : x.Id == LocalState.CurrentUser.Id).OrderBy(x => LocalState.Guilds[raw.GuildId].roles[x.Id].Name == "@everyone").ThenBy(x => x.Type == "role"))
            {
                permissions.AddAllows(overwrite.Allow);
                permissions.AddDenies(overwrite.Deny);
            }
        }

        public SharedModels.GuildChannel raw;
        public Permissions permissions;
    }
}
