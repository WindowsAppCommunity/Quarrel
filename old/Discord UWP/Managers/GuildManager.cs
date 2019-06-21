using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;
using Quarrel.SimpleClasses;

using Guild = DiscordAPI.SharedModels.Guild;

namespace Quarrel.Managers
{
    class GuildManager
    {

        public static SimpleGuild CreateGuild(Guild guild)
        {
            var sg = new SimpleGuild()
            {
                Id = guild.Id,
                Name = guild.Name,
                ImageURL = "https://cdn.discordapp.com/icons/" + guild.Id + "/" + guild.Icon + ".png",
                IsDM = false,
                IsMuted = LocalState.GuildSettings.ContainsKey(guild.Id) ? LocalState.GuildSettings[guild.Id].raw.Muted : false,
                IsUnread = false, //Will Change if true
                IsValid = true, //Will change if false
                IsSelected = false
            };

            foreach (var chn in LocalState.Guilds[guild.Id].channels.Values)
                if (LocalState.RPC.ContainsKey(chn.raw.Id))
                {
                    ReadState readstate = LocalState.RPC[chn.raw.Id];
                    sg.NotificationCount += readstate.MentionCount;
                    var StorageChannel = LocalState.Guilds[sg.Id].channels[chn.raw.Id].raw;
                    if (readstate.LastMessageId != StorageChannel.LastMessageId && !sg.IsMuted)
                        sg.IsUnread = true;
                }
            return sg;
        }
        public static SimpleGuild CreateGuild(UserGuild guild)
        {
             return new SimpleGuild()
            {
                Id = guild.Id,
                Name = guild.Name,
                ImageURL = "https://cdn.discordapp.com/icons/" + guild.Id + "/" + guild.Icon + ".png",
                IsDM = false,
                IsMuted = LocalState.GuildSettings.ContainsKey(guild.Id) ? LocalState.GuildSettings[guild.Id].raw.Muted : false,
                IsUnread = false, //Will Change if true
                IsValid = true, //Will change if false
                IsSelected = false
            };
        }
    }
}
