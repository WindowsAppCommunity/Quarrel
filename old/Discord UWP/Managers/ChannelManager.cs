using System.Collections.Generic;
using System.Linq;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;
using Quarrel.SimpleClasses;
using GuildChannel = DiscordAPI.SharedModels.GuildChannel;

namespace Quarrel.Managers
{
    public static class ChannelManager
    {
        /// <summary>
        /// Make SimpleChannel based on GuildChannel
        /// </summary>
        /// <param name="channel">GuildChannel</param>
        /// <returns>SimpleChannel</returns>
        public static SimpleChannel MakeChannel(LocalModels.GuildChannel channel)
        {
            // Create basic SimpleChannel
            SimpleChannel sc = new SimpleChannel
            {
                Id = channel.raw.Id,
                Name = channel.raw.Name,
                Type = channel.raw.Type,
                Nsfw = channel.raw.NSFW,
                Position = channel.raw.Position,
                ParentId = channel.raw.ParentId,
                Icon = channel.raw.Icon
            };

            switch (channel.raw.Type)
            {
                // Text Channel
                case 0:
                    // Determine muted status
                    sc.IsMuted = LocalState.GuildSettings.ContainsKey(channel.raw.GuildId) &&
                                 (LocalState.GuildSettings[channel.raw.GuildId].channelOverrides
                                      .ContainsKey(channel.raw.Id) && LocalState.GuildSettings[channel.raw.GuildId]
                                      .channelOverrides[channel.raw.Id].Muted);

                    // Determine is unread and notification count
                    if (LocalState.RPC.ContainsKey(sc.Id))
                    {
                        ReadState readstate = LocalState.RPC[sc.Id];
                        sc.NotificationCount = readstate.MentionCount;
                        var StorageChannel = LocalState.Guilds[App.CurrentGuildId].channels[sc.Id];
                        sc.IsUnread = StorageChannel?.raw.LastMessageId != null && readstate.LastMessageId != StorageChannel.raw.LastMessageId;
                    }

                    // Assing if user has read permissions
                    sc.HavePermissions = 
                        LocalState.Guilds[App.CurrentGuildId].channels[sc.Id].permissions.ReadMessages 
                        || App.CurrentGuildId == sc.Id;

                    // Determine if SimpleChannel should be shown
                    if (!(sc.IsMuted && Storage.Settings.HideMutedChannels) && (sc.HavePermissions || Storage.Settings.ShowNoPermissionChannels))
                    {
                        return sc;
                    }
                    break;
                
                // Voice channel
                case 2:
                    sc.HavePermissions = 
                        LocalState.Guilds[App.CurrentGuildId].channels[sc.Id].permissions.Connect 
                        || App.CurrentGuildId == sc.Id;
                    if (!(sc.IsMuted && Storage.Settings.HideMutedChannels) && (sc.HavePermissions || Storage.Settings.ShowNoPermissionChannels))
                    {
                        return sc;
                    }
                    break;
                
                // Category channel
                case 4:
                    //TODO: Categories
                    break;
            }

            return null;
        }

        /// <summary>
        /// Make SimpleChannel from GuildChannel with overriden LastMessageId
        /// </summary>
        /// <param name="channel">GuildChannel</param>
        /// <param name="overridelastmessageid">New LastMessageId</param>
        /// <returns>SimpleChannel</returns>
        public static SimpleChannel MakeChannel(GuildChannel channel, string overridelastmessageid = null)
        {
            // Create basic SimpleChannel
            SimpleChannel sc = new SimpleChannel
            {
                Id = channel.Id,
                Name = channel.Name,
                Type = channel.Type,
                Nsfw = channel.NSFW,
                // Override LastMessageId
                LastMessageId = overridelastmessageid ?? channel.LastMessageId,
                Position = channel.Position,
                ParentId = channel.ParentId,
                Icon = channel.Icon
            };

            // Determine if has permission
            sc.HavePermissions = 
                LocalState.Guilds[App.CurrentGuildId].channels[sc.Id].permissions.ReadMessages
                || App.CurrentGuildId == sc.Id;

            // Determine if SimpleChannel should be displayed
            if (!(sc.IsMuted && Storage.Settings.HideMutedChannels) && (sc.HavePermissions || Storage.Settings.ShowNoPermissionChannels))
            {
                return sc;
            }

            return null;
        }

        /// <summary>
        /// Make SimpleChannel from DMChannel
        /// </summary>
        /// <param name="channel">DMChannel</param>
        /// <returns>SimpleChannel</returns>
        public static SimpleChannel MakeChannel(DirectMessageChannel channel)
        {
            // Create basic SimpleChannel
            SimpleChannel sc = new SimpleChannel();
            sc.Id = channel.Id;
            sc.Type = channel.Type;
           
            switch (channel.Type)
            {
                // DM
                case 1:
                    // Assign basic DM info
                    sc.Name = "@" + channel.Users.FirstOrDefault().Username;
                    sc.LastMessageId = channel.LastMessageId;
                    sc.ImageURL = "https://cdn.discordapp.com/avatars/" + channel.Users.FirstOrDefault().Id + "/" + channel.Users.FirstOrDefault().Avatar + ".png?size=64";

                    // Add members
                    sc.Members = new Dictionary<string, User>();
                    foreach (User user in channel.Users)
                    {
                        sc.Members.Add(user.Id, user);
                    }

                    // Determine presence
                    if (LocalState.PresenceDict.ContainsKey(channel.Users.FirstOrDefault().Id))
                    {
                        sc.UserStatus = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id];
                        sc.Playing = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game;
                        if (LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game != null)
                        {
                            sc.Playing = new Game()
                            {
                                Name = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game
                                .Name,
                                Type = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game.Type,
                                Url = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game.Url
                            };
                        }
                    }
                    else
                    {
                        sc.UserStatus = new Presence() { Status = "offline" };
                    }

                    //sc.IsMuted = LocalState.GuildSettings.ContainsKey(channel.raw.GuildId) ? (LocalState.GuildSettings[channel.raw.GuildId].channelOverrides.ContainsKey(channel.raw.Id) ? LocalState.GuildSettings[channel.raw.GuildId].channelOverrides[channel.raw.Id].Muted : false) : false;
                    if (LocalState.RPC.ContainsKey(sc.Id))
                    {
                        ReadState readstate = LocalState.RPC[sc.Id];
                        sc.NotificationCount = readstate.MentionCount;
                        var StorageChannel = LocalState.DMs[sc.Id];
                        if (StorageChannel.LastMessageId != null &&
                            readstate.LastMessageId != StorageChannel.LastMessageId)
                            sc.IsUnread = true;
                        else
                            sc.IsUnread = false;
                    }
                    return sc;

                // Group DM
                case 3:
                    // Assign basic Group DM info
                    sc.Name = channel.Name;
                    sc.LastMessageId = channel.LastMessageId;
                    sc.Subtitle = App.GetString("/Main/members").Replace("<count>", (channel.Users.Count() + 1).ToString());
                    sc.Icon = channel.Icon;

                    // Override null name
                    if (!string.IsNullOrEmpty(channel.Name))
                    {
                        sc.Name = channel.Name;
                    }
                    else
                    {
                        List<string> channelMembers = new List<string>();
                        foreach (var d in channel.Users)
                            channelMembers.Add(d.Username);
                        sc.Name = string.Join(", ", channelMembers);
                    }

                    // Determine Unread and Notification status
                    if (LocalState.RPC.ContainsKey(sc.Id))
                    {
                        ReadState readstate = LocalState.RPC[sc.Id];
                        sc.NotificationCount = readstate.MentionCount;
                        var StorageChannel = LocalState.DMs[sc.Id];
                        if (StorageChannel.LastMessageId != null &&
                            readstate.LastMessageId != StorageChannel.LastMessageId)
                            sc.IsUnread = true;
                        else
                            sc.IsUnread = false;
                    }
                    
                    // Show SimpleChannel
                    sc.HavePermissions = true;
                    return sc;
            }
            return null;
        }

        /// <summary>
        /// Make a List of SimpleChannels from a List of GuildChannels
        /// </summary>
        /// <param name="channels">List of GuildChannels</param>
        /// <returns>List of SimpleChannels</returns>
        public static List<SimpleChannel> OrderChannels(List<LocalModels.GuildChannel> channels)
        {
            // Create return list
            List<SimpleChannel> returnChannels = new List<SimpleChannel>();

            // For each channel
            foreach (var channel in channels)
            {
                // Create basic SimpleChannel
                SimpleChannel sc = new SimpleChannel();
                sc.Id = channel.raw.Id;
                sc.Name = channel.raw.Name;
                sc.Type = channel.raw.Type;
                sc.Position = channel.raw.Position;
                sc.ParentId = channel.raw.ParentId;
                sc.Nsfw = channel.raw.NSFW;

                switch (channel.raw.Type)
                {
                    // Text Channel
                    case 0:
                        // Determine muted
                        sc.IsMuted = LocalState.GuildSettings.ContainsKey(channel.raw.GuildId) && (LocalState.GuildSettings[channel.raw.GuildId].channelOverrides.ContainsKey(channel.raw.Id) && LocalState.GuildSettings[channel.raw.GuildId].channelOverrides[channel.raw.Id].Muted);

                        // Determine Unread and Notification status
                        if (LocalState.RPC.ContainsKey(sc.Id))
                        {
                            ReadState readstate = LocalState.RPC[sc.Id];
                            sc.NotificationCount = readstate.MentionCount;
                            var storageChannel = LocalState.CurrentGuild.channels[sc.Id];
                            sc.IsUnread = storageChannel?.raw.LastMessageId != null &&
                                readstate.LastMessageId != storageChannel.raw.LastMessageId;
                        }

                        // Detemine if user has read permissions
                        sc.HavePermissions = 
                            LocalState.CurrentGuild.channels[sc.Id].permissions.ReadMessages 
                            || App.CurrentGuildId == sc.Id;

                        // Determine if user should see channel
                        if (!(sc.IsMuted && Storage.Settings.HideMutedChannels) && (sc.HavePermissions || Storage.Settings.ShowNoPermissionChannels))
                        {
                            returnChannels.Add(sc);
                        }
                        break;

                    // Voice Channel
                    case 2:
                        // Determine if user has Connect permissions
                        sc.HavePermissions = 
                            LocalState.CurrentGuild.channels[sc.Id].permissions.Connect 
                            || App.CurrentGuildId == sc.Id;

                        // Determine if user should see channel
                        if (!(sc.IsMuted && Storage.Settings.HideMutedChannels) && (sc.HavePermissions || Storage.Settings.ShowNoPermissionChannels))
                        {
                            returnChannels.Add(sc);
                        }
                        break;

                    // Category
                    case 4:
                        sc.HavePermissions = 
                            LocalState.CurrentGuild.channels[sc.Id].permissions.ReadMessages 
                            || LocalState.CurrentGuild.channels[sc.Id].permissions.Connect;
                        sc.Name = sc.Name.ToUpper();

                        if (!(sc.IsMuted && Storage.Settings.HideMutedChannels) && (sc.HavePermissions || Storage.Settings.ShowNoPermissionChannels))
                        {
                            returnChannels.Add(sc);
                        }
                        break;
                }
            }
            
            var Categorized = returnChannels.Where(x => x.ParentId != null && x.Type != 4).OrderBy(x => x.Type).ThenBy(x => x.Position);
            var Categories = returnChannels.Where(x => x.Type == 4).OrderBy(x => x.Position).ToList();
            List<SimpleChannel> Sorted = new List<SimpleChannel>();
            foreach (var noId in returnChannels.Where(x => x.ParentId == null && x.Type != 4).OrderBy(x => x.Type).ThenBy(x => x.Position))
                Sorted.Add(noId);
            foreach(var categ in Categories)
            {
                if (Categorized.Where(x => x.ParentId == categ.Id).Count() > 0)
                {
                    Sorted.Add(categ);
                    foreach (var item in Categorized.Where(x => x.ParentId == categ.Id))
                        Sorted.Add(item);
                }
            }
            return Sorted;
        }

        /// <summary>
        /// Create a List of SimpleChannels from a List of DirectMessageChannels
        /// </summary>
        /// <param name="channels">List of DirectMessageChannels</param>
        /// <returns>List of SimpleChannels</returns>
        public static List<SimpleChannel> OrderChannels(List<DirectMessageChannel> channels)
        {
            // Create return list
            List<SimpleChannel> returnChannels = new List<SimpleChannel>();

            foreach (var channel in channels)
            {
                SimpleChannel sc = new SimpleChannel();
                sc.Id = channel.Id;
                sc.Type = channel.Type;
                switch (channel.Type)
                {
                    // DM
                    case 1:
                        // Assign basic DM info
                        sc.Name = "@" + channel.Users.FirstOrDefault().Username;
                        sc.UserId = channel.Users.FirstOrDefault().Id;
                        sc.LastMessageId = channel.LastMessageId;
                        sc.ImageURL = "https://cdn.discordapp.com/avatars/" + channel.Users.FirstOrDefault().Id + "/" + channel.Users.FirstOrDefault().Avatar + ".png?size=64";

                        // Add members 
                        sc.Members = new Dictionary<string, User>();
                        foreach (User user in channel.Users)
                        {
                            sc.Members.Add(user.Id, user);
                        }

                        // Determine presence
                        if (LocalState.PresenceDict.ContainsKey(channel.Users.FirstOrDefault().Id))
                        {
                            sc.UserStatus = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id];
                            sc.Playing = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game;
                            if (LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game != null)
                            {
                                sc.Playing = new Game()
                                {
                                    Name = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game
                                    .Name,
                                    Type = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game.Type,
                                    Url = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game.Url
                                };
                            }
                        }
                        else
                        {
                            sc.UserStatus = new Presence() { Status = "offline" };
                        }


                        // Determine unread and notifcation status
                        if (LocalState.RPC.ContainsKey(sc.Id))
                        {
                            ReadState readstate = LocalState.RPC[sc.Id];
                            sc.NotificationCount = readstate.MentionCount;
                            var StorageChannel = LocalState.DMs[sc.Id];
                            if (StorageChannel.LastMessageId != null &&
                                readstate.LastMessageId != StorageChannel.LastMessageId)
                                sc.IsUnread = true;
                            else
                                sc.IsUnread = false;
                        }

                        // Add to return channel list
                        sc.HavePermissions = true;
                        returnChannels.Add(sc);
                        break;

                    // Group DM
                    case 3:
                        // Assign basic Group info
                        sc.Name = channel.Name;
                        sc.LastMessageId = channel.LastMessageId;
                        sc.Subtitle = App.GetString("/Main/members").Replace("<count>", (channel.Users.Count() + 1).ToString());
                        sc.Icon = channel.Icon;

                        // Override group name
                        if (channel.Name != null && channel.Name != "")
                        {
                            sc.Name = channel.Name;
                        }
                        else
                        {
                            List<string> channelMembers = new List<string>();
                            foreach (var d in channel.Users)
                                channelMembers.Add(d.Username);
                            sc.Name = string.Join(", ", channelMembers);
                        }

                        // Determine unread and notification status
                        if (LocalState.RPC.ContainsKey(sc.Id))
                        {
                            ReadState readstate = LocalState.RPC[sc.Id];
                            sc.NotificationCount = readstate.MentionCount;
                            var StorageChannel = LocalState.DMs[sc.Id];
                            if (StorageChannel.LastMessageId != null &&
                                readstate.LastMessageId != StorageChannel.LastMessageId)
                                sc.IsUnread = true;
                            else
                                sc.IsUnread = false;
                        }

                        // Add channel to return list
                        sc.HavePermissions = true;
                        returnChannels.Add(sc);
                        break;
                }
            }

            // Order by LastMessageId
            return returnChannels.OrderByDescending(x => x.LastMessageId).ToList();
        }
    }
}
