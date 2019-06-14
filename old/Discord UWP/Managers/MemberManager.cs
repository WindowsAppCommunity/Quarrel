using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using DiscordAPI.SharedModels;
using System.ComponentModel;
using Quarrel.LocalModels;
using Quarrel.SimpleClasses;

namespace Quarrel.Managers
{
    class MemberManager
    {
        private static Dictionary<string, HoistRole> TempRoleCache = new Dictionary<string, HoistRole>(); //This is as a temporary cache of roles to improve performance and not call Storage for every member
        public static HoistRole GetRole(string roleid, string guildid)
        {
            string dicrole = roleid ?? "0";
            if (TempRoleCache.ContainsKey(dicrole))
            {
                //TempRoleCache[dicrole].Membercount++;
                return TempRoleCache[dicrole];
            }
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
                    role = new HoistRole(roleid, storageRole.Position, storageRole.Name.ToUpper(), 0, storageRole.Color);
                    TempRoleCache.Add(dicrole, role);
                }
                return role;
            }
        }
        public static void ClearRoles()
        {
            TempRoleCache.Clear();
        }
    }
}
