using CommonServiceLocator;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Quarrel.Models.Bindables;
using Quarrel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Helpers;
using DiscordAPI.Models;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    internal static class ReadyExtenions
    {
        public static void Cache(this Ready ready)
        {
            #region Settings

            foreach (var gSettings in ready.GuildSettings)
            {
                ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, gSettings, gSettings.GuildId);

                foreach (var cSettings in gSettings.ChannelOverrides)
                {
                    ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.ChannelSettings, cSettings, cSettings.ChannelId);
                }
            }

            #endregion



            #region Guilds and Channels

            List<BindableGuild> guildList = new List<BindableGuild>();
            foreach (var guild in ready.Guilds)
            {
                BindableGuild bGuild = new BindableGuild(guild);

                // Handle guild settings
                GuildSetting gSettings = ServicesManager.Cache.Runtime.TryGetValue<GuildSetting>(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, guild.Id);
                if (gSettings != null)
                {
                    bGuild.Muted = gSettings.Muted;
                }

                // Guild Order
                bGuild.Position = ready.Settings.GuildOrder.IndexOf(x => x == bGuild.Model.Id);

                List<BindableChannel> channelList = new List<BindableChannel>();
                foreach (var channel in guild.Channels)
                {
                    BindableChannel bChannel = new BindableChannel(channel);

                    // Handle channel settings
                    ChannelOverride cSettings = ServicesManager.Cache.Runtime.TryGetValue<ChannelOverride>(Quarrel.Helpers.Constants.Cache.Keys.ChannelSettings, channel.Id);
                    if (cSettings != null)
                    {
                        bChannel.Muted = cSettings.Muted;
                    }

                    // Find parent position
                    if (!string.IsNullOrEmpty(bChannel.ParentId))
                        bChannel.ParentPostion = guild.Channels.First(x => x.Id == bChannel.ParentId).Position;

                    channelList.Add(bChannel);
                }

                channelList = channelList.OrderBy(x => x.AbsolutePostion).ToList();

                ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.ChannelList, channelList, guild.Id);

                bGuild.Model.Channels = null;

                guildList.Add(bGuild);
            }

            guildList = guildList.OrderBy(x => x.Position).ToList();

            #endregion

            ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildList, guildList);
        }
    }
}
