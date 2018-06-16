using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using System.ComponentModel;
using Discord_UWP.SimpleClasses;

namespace Discord_UWP.Managers
{
    class MemberManager
    {
        private static Dictionary<string, HoistRole> TempRoleCache = new Dictionary<string, HoistRole>(); //This is as a temporary cache of roles to improve performance and not call Storage for every member
        public static HoistRole GetRole(string roleid, string guildid)
        {
            string dicrole = roleid == null ? "0" : roleid;
            if (TempRoleCache.ContainsKey(dicrole))
                return TempRoleCache[dicrole];
            else
            {
                HoistRole role;
                if (roleid == null || !LocalState.Guilds[guildid].roles[roleid].Hoist)
                {
                    role = new HoistRole(null, 0, App.GetString("/Main/Everyone"), 0, -1);
                    TempRoleCache.Add(dicrole, role);
                }
                else
                {
                    var storageRole = LocalState.Guilds[guildid].roles[roleid];
                    role = new HoistRole(roleid, storageRole.Position, storageRole.Name.ToUpper(), storageRole.MemberCount, storageRole.Color);
                    TempRoleCache.Add(dicrole, role);
                }
                return role;
            }
        }
    }


}
