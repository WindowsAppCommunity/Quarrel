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
    public static class MentionsConverter
    {
        private static IChannelsService _channelsService;
        private static IGuildsService _guildsService ;

        private static IChannelsService ChannelsService => _channelsService ?? (_channelsService = SimpleIoc.Default.GetInstance<IChannelsService>());
        private static IGuildsService GuildsService => _guildsService ?? (_guildsService = SimpleIoc.Default.GetInstance<IGuildsService>());

        public static IDictionary<string, (string, int)> UserMentionsConverter(IEnumerable<User> Users)
        {
            return Users.ToDictionary(x => x.Id, x => (x.Username, GuildsService.GetGuildMember(x.Id, GuildsService.CurrentGuild.Model.Id)?.TopRole?.Color ?? 0x18363));
        }

        public static IDictionary<string, (string, int)> RoleMentionsConverter(IEnumerable<string> roles)
        {
            if (roles != null)
            {
                IDictionary<string, (string, int)> dict = new Dictionary<string, (string, int)>();
                foreach (string roleId in roles)
                {
                    var role = GuildsService.CurrentGuild.Model.Roles.FirstOrDefault(x => x.Id == roleId);
                    if (role != null)
                    {
                        dict.Add(roleId, (role.Name, role.Color));
                    }
                }

                return dict;
            }

            return null;
        }

        public static IDictionary<string, string> ChannelMentionsConverter(IEnumerable<ChannelMention> channels)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var channel in ChannelsService.AllChannels)
            {
                dict[channel.Key] = channel.Value.Model.Name;
            }

            if (channels != null)
            {
                foreach (var channel in channels)
                {
                    dict[channel.Id] = channel.Name;
                }
            }

            return dict;
        }
    }
}
