using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.CacheModels
{
    public class Friend
    {
        public Friend(SharedModels.Friend input)
        {
            Raw = input;
            //avatar.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + input.Id + "/" + input.Avatar + ".jpg"));
        }

        public Friend(TempFriend input)
        {
            Raw = input.Raw;

            //avatar = input.avatar;
        }

        public SharedModels.Friend Raw = new SharedModels.Friend();
    }
}
