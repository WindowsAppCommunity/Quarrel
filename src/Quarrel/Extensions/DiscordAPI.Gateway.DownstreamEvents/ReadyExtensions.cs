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
    internal static class ReadyExtentions
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


            // Add DM
            var dmGuild = new BindableGuild(new Guild() { Name = "DM", Id = "DM" });
            guildList.Add(dmGuild);

            // Add DM channels
            if (ready.PrivateChannels != null && ready.PrivateChannels.Count() > 0)
            {
                List<BindableChannel> channelList = new List<BindableChannel>();

                foreach (var channel in ready.PrivateChannels)
                {
                    BindableChannel bChannel = new BindableChannel(channel);
                    channelList.Add(bChannel);
                }

                // Sort by last message timestamp
                channelList = channelList.OrderByDescending(x => Convert.ToUInt64(x.Model.LastMessageId)).ToList();

                ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.ChannelList, channelList, "DM");
            }


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

                // Guild Channels
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

                    ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Channel, bChannel, bChannel.Model.Id);
                    channelList.Add(bChannel);
                }

                channelList = channelList.OrderBy(x => x.AbsolutePostion).ToList();

                ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.ChannelList, channelList, guild.Id);

                bGuild.Model.Channels = null;

                // Guild Members (Step #1)
                // Full list from GuildMemberChunk
                foreach (var user in guild.Members)
                {
                    BindableGuildMember bgMember = new BindableGuildMember(user);
                    bgMember.GuildId = guild.Id;
                    ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildMember, bgMember, guild.Id + user.User.Id);
                }

                // Guild Roles
                foreach (var role in guild.Roles)
                {
                    ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildRole, role, guild.Id + role.Id);
                }

                guildList.Add(bGuild);
                ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Guild, bGuild, bGuild.Model.Id);
            }

            guildList = guildList.OrderBy(x => x.Position).ToList();

            ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildList, guildList);

            #endregion

            #region ReadStates

            foreach (var state in ready.ReadStates)
            {
                ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.ReadState, state, state.Id);
            }

            #endregion
        }
    }
}
