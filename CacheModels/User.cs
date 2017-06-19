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
    public class User
    {
        public User(SharedModels.User input)
        {
            Raw = input;
            //avatar.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + input.Id + "/" + input.Avatar + ".jpg"));
        }

        public User(TempUser input)
        {
            Raw = input.Raw;

            //avatar = input.avatar;
        }

        public SharedModels.User Raw = new SharedModels.User();

        //public string token;
        //public ImageBrush avatar = new ImageBrush();
    }
}
