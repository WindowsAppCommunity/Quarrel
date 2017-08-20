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
        public Member()
        {

        }

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

        public SharedModels.GuildMember Raw;
        //public ImageBrush avatar = new ImageBrush();

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
