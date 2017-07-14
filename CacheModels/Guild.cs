using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Discord_UWP.CacheModels
{
    public class Guild
    {
        public Guild(SharedModels.UserGuild input)
        {
            GetGuild(input.Id);
        }

        public Guild(SharedModels.Guild input)
        {
            RawGuild = input;

        }

        public async void GetGuild(string id)
        {
            RawGuild = await Session.GetGuild(id);
        }

        public Guild(TempGuild input)
        {
            RawGuild = new SharedModels.Guild();
            RawGuild.Id = input.Id;
            RawGuild.Name = input.Name;
            RawGuild.Icon = input.Icon;
            RawGuild.OwnerId = input.OwnerId;
            RawGuild.Splash = input.Splash;
            RawGuild.Region = input.Region;
            RawGuild.Roles = input.Roles.AsEnumerable();

            foreach (TempGuildChannel channel in input.Channels)
            {
                Channels.Add(channel.Id, new GuildChannel(channel));
            }

            foreach (TempMember user in input.Members)
            {
                Members.Add(user.User.Id, new Member(user));
            }

            foreach (SharedModels.Role role in input.Roles)
            {
                Roles.Add(role.Id, role);
            }
        }
        
        public SharedModels.Guild RawGuild = new SharedModels.Guild();

        public Dictionary<string, SharedModels.Role> Roles = new Dictionary<string, SharedModels.Role>();
        public Dictionary<string, GuildChannel> Channels = new Dictionary<string, GuildChannel>();
        public Dictionary<string, Member> Members = new Dictionary<string, Member>();
        public Common.Permissions perms = new Common.Permissions();
    }
}
