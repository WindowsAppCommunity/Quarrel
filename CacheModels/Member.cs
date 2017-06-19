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
    public class Member
    {
        public Member(SharedModels.GuildMember input)
        {
            Raw = input;
            //avatar = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + input.User.Id + "/" + input.User.Avatar + ".jpg")) };
        }

        public Member(TempMember input)
        {
            Raw.User = input.User;
            Raw.Nick = input.Nick;
            Raw.Roles = input.Roles.AsEnumerable();
            Raw.JoinedAt = input.JoinedAt;
            Raw.Deaf = input.Deaf;
            Raw.Mute = input.Mute;
            //avatar = input.avatar;
        }

        public SharedModels.GuildMember Raw = new SharedModels.GuildMember();
        //public ImageBrush avatar = new ImageBrush();
    }
}
