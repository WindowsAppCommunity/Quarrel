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
            //GetPermissions(); Called to early
        }

        public GuildChannel(SharedModels.GuildChannel channel, string guildId)
        {
            raw = channel;
            raw.GuildId = guildId;
            //GetPermissions(); Called too early
        }

        public void GetPermissions()
        {
            permissions = LocalState.Guilds[raw.GuildId].permissions;
            if (raw.ParentId != null)
            {
                foreach (Overwrite overwrite in LocalState.Guilds[raw.GuildId].channels[raw.ParentId].raw.PermissionOverwrites.TakeWhile(x => x.Type == "role" ? LocalState.Guilds[raw.GuildId].members[LocalState.CurrentUser.Id].Roles.Contains(x.Id) : x.Id == LocalState.CurrentUser.Id).OrderBy(x => LocalState.Guilds[raw.GuildId].roles[x.Id].Name == "@everyone").ThenBy(x => x.Type == "role"))
                {
                    permissions.AddAllows(overwrite.Allow);
                    permissions.AddDenies(overwrite.Deny);
                }
            }

            foreach (Overwrite overwrite in raw.PermissionOverwrites.TakeWhile(x => x.Type == "role" ? LocalState.Guilds[raw.GuildId].members[LocalState.CurrentUser.Id].Roles.Contains(x.Id) : x.Id == LocalState.CurrentUser.Id).OrderBy(x => LocalState.Guilds[raw.GuildId].roles.ContainsKey(x.Id) ? LocalState.Guilds[raw.GuildId].roles[x.Id].Name == "@everyone" : true).ThenBy(x => x.Type == "role"))
            {
                permissions.AddAllows(overwrite.Allow);
                permissions.AddDenies(overwrite.Deny);
            }
        }

        public SharedModels.GuildChannel raw;
        public Permissions permissions;
    }
}
