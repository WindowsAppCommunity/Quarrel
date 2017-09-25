using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.CacheModels
{
    class CurrentUser
    {
        public CurrentUser(SharedModels.User input)
        {
            Raw = input;
            //avatar.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + input.Id + "/" + input.Avatar + ".jpg"));
        }

        public CurrentUser(TempCurrentUser input)
        {
            Raw = input.Raw;
            /*foreach ()
            {

            }*/
            //avatar = input.avatar;
        }

        public SharedModels.User Raw = new SharedModels.User();
        //public Dictionary<string, CacheModels.User> Friends = new Dictionary<string, User>();
    }
}
