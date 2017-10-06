using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Managers
{
    class MemberManager
    {
        private static List<DisplayedRole> TempRoleCache = new List<DisplayedRole>(); //This is as a temporary cache of roles to improve performance and not call Storage for every member
        public static DisplayedRole GetRole(string roleid, string guildid, int everyonecounter)
        {
            var cachedRole = TempRoleCache.FirstOrDefault(x => x.Id == roleid);
            if (cachedRole != null) return cachedRole;
            else
            {
                DisplayedRole role;
                if (roleid == null || !LocalState.Guilds[guildid].roles[roleid].Hoist)
                {

                    role = new DisplayedRole(null, 0, App.GetString("/Main/Everyone"), everyonecounter, (SolidColorBrush)App.Current.Resources["Foreground"]);
                    TempRoleCache.Add(role);
                }
                else
                {
                    var storageRole = LocalState.Guilds[guildid].roles[roleid];
                    role = new DisplayedRole(roleid, storageRole.Position, storageRole.Name.ToUpper(), storageRole.MemberCount, Common.IntToColor(storageRole.Color));
                    TempRoleCache.Add(role);
                }
                return role;
            }
        }
    }

    public class Member
    {
        public Member(SharedModels.GuildMember input)
        {
            Raw = input;
            //avatar = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + input.User.Id + "/" + input.User.Avatar + ".jpg")) };
        }

        public SharedModels.GuildMember Raw;

        public SharedModels.Role HighRole = new SharedModels.Role();
        public bool IsTyping { get; set; }
        public DisplayedRole MemberDisplayedRole { get; set; }
        public SharedModels.Presence status { get; set; }
        public SharedModels.VoiceState voicestate { get; set; }
    }

    public class DisplayedRole
    {
        public string Id { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public int Membercount { get; set; }
        public SolidColorBrush Brush { get; set; }

        public DisplayedRole(string id, int position, string name, int membercount, SolidColorBrush brush)
        { Id = id; Position = position; Name = name; Membercount = membercount; Brush = brush; }
    }
}
