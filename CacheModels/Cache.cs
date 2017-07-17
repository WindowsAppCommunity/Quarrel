using Discord_UWP.CacheModels;
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

namespace Discord_UWP
{
    public class Cache
    {
        public Cache(){}
        public Cache(TempCache input)
        {
            CurrentUser = new User(input.CurrentUser);
            foreach (TempGuild guild in input.Guilds)
            {
                try
                {
                    Guilds.Add(guild.Id, new Guild(guild));
                }
                catch (Exception) { }
            }

            foreach(TempDmCache dm in input.DMs)
            {
                DMs.Add(dm.Id, new DmCache(dm));
            }

            foreach (TempFriend friend in input.Friends)
            {
                Friends.Add(friend.Raw.Id, new Friend(friend));
            }
        }

        public User CurrentUser;
        //public Dictionary<string, int> guildOrder = new Dictionary<string, int>();
        public Dictionary<string, Friend> Friends = new Dictionary<string, Friend>();
        public Dictionary<string, Guild> Guilds = new Dictionary<string, Guild>();
        public Dictionary<string, DmCache> DMs = new Dictionary<string, DmCache>();
    }
}
