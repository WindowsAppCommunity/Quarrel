using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.Guilds;

namespace Quarrel.Converters.Messages
{
    public class MentionsConverter
    {
        private static IGuildsService _guildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
        public static IDictionary<string, string> UserMentionsConverter(IEnumerable<User> Users)
        {
            return Users.ToDictionary(x => x.Id, x => x.Username);
        }
    }
}
