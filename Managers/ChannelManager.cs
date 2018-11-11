using System.Collections.Generic;
using System.Linq;
using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using Discord_UWP.SimpleClasses;

namespace Discord_UWP.Managers
{
    public static class ChannelManager
    {
        public static SimpleChannel MakeChannel(LocalModels.GuildChannel channel)
        {
            SimpleChannel sc = new SimpleChannel();
            sc.Id = channel.raw.Id;
            sc.Name = channel.raw.Name;
            sc.Type = channel.raw.Type;
            sc.Nsfw = channel.raw.NSFW;
            sc.Position = channel.raw.Position;
            sc.ParentId = channel.raw.ParentId;
            sc.Icon = channel.raw.Icon;
            switch (channel.raw.Type)
            {
                case 0:
                    sc.IsMuted = LocalState.GuildSettings.ContainsKey(channel.raw.GuildId) && (LocalState.GuildSettings[channel.raw.GuildId].channelOverrides.ContainsKey(channel.raw.Id) && LocalState.GuildSettings[channel.raw.GuildId].channelOverrides[channel.raw.Id].Muted);
                    if (LocalState.RPC.ContainsKey(sc.Id))
                    {
                        ReadState readstate = LocalState.RPC[sc.Id];
                        sc.NotificationCount = readstate.MentionCount;
                        var StorageChannel = LocalState.Guilds[App.CurrentGuildId].channels[sc.Id];
                        sc.IsUnread = StorageChannel?.raw.LastMessageId != null && readstate.LastMessageId != StorageChannel.raw.LastMessageId;
                    }
                    if (LocalState.Guilds[App.CurrentGuildId].channels[sc.Id].permissions.ReadMessages || App.CurrentGuildId == sc.Id)
                    {
                        return sc;
                    }
                    break;
                case 2:
                    if (LocalState.Guilds[App.CurrentGuildId].channels[sc.Id].permissions.Connect || App.CurrentGuildId == sc.Id)
                    {
                        return sc;
                    }
                    break;
                case 4:
                    //TODO: Categories
                    break;
            }
            return null;
        }
        public static SimpleChannel MakeChannel(SharedModels.GuildChannel channel, string overridelastmessageid = null)
        {
            SimpleChannel sc = new SimpleChannel();
            sc.Id = channel.Id;
            sc.Name = channel.Name;
            sc.Type = channel.Type;
            sc.Nsfw = channel.NSFW;
            if (overridelastmessageid == null)
                sc.LastMessageId = channel.LastMessageId;
            else
                sc.LastMessageId = overridelastmessageid;
            sc.Position = channel.Position;
            sc.ParentId = channel.ParentId;
            sc.Icon = channel.Icon;
            return sc;

        }
        public static SimpleChannel MakeChannel(SharedModels.DirectMessageChannel channel)
        {
            SimpleChannel sc = new SimpleChannel();
            sc.Id = channel.Id;
            sc.Type = channel.Type;
           
            switch (channel.Type)
            {
                case 1: //DM
                    sc.Name = "@" + channel.Users.FirstOrDefault().Username;
                    sc.LastMessageId = channel.LastMessageId;
                    sc.ImageURL = "https://cdn.discordapp.com/avatars/" + channel.Users.FirstOrDefault().Id + "/" + channel.Users.FirstOrDefault().Avatar + ".png?size=64";

                    sc.Members = new Dictionary<string, User>();
                    foreach (User user in channel.Users)
                    {
                        sc.Members.Add(user.Id, user);
                    }

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
                case 3: //Group
                    sc.Name = channel.Name;
                    sc.LastMessageId = channel.LastMessageId;
                    sc.Subtitle = App.GetString("/Main/members").Replace("<count>", (channel.Users.Count() + 1).ToString());
                    sc.Icon = channel.Icon;
                    //sc.Members = new Dictionary<string, User>();
                    //foreach (User user in channel.Users)
                    //{
                    //    sc.Members.Add(user.Id, user);
                    //}

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
            }
            return null;
        }

        public static List<SimpleChannel> OrderChannels(List<LocalModels.GuildChannel> channels)
        {
            List<SimpleChannel> returnChannels = new List<SimpleChannel>();
            foreach (var channel in channels)
            {
                SimpleChannel sc = new SimpleChannel();
                sc.Id = channel.raw.Id;
                sc.Name = channel.raw.Name;
                sc.Type = channel.raw.Type;
                sc.Position = channel.raw.Position;
                sc.ParentId = channel.raw.ParentId;
                sc.Nsfw = channel.raw.NSFW;

                switch (channel.raw.Type)
                {
                    case 0:
                        sc.IsMuted = LocalState.GuildSettings.ContainsKey(channel.raw.GuildId) && (LocalState.GuildSettings[channel.raw.GuildId].channelOverrides.ContainsKey(channel.raw.Id) && LocalState.GuildSettings[channel.raw.GuildId].channelOverrides[channel.raw.Id].Muted);
                        if (LocalState.RPC.ContainsKey(sc.Id))
                        {
                            ReadState readstate = LocalState.RPC[sc.Id];
                            sc.NotificationCount = readstate.MentionCount;
                            var storageChannel = LocalState.CurrentGuild.channels[sc.Id];
                            sc.IsUnread = storageChannel?.raw.LastMessageId != null &&
                                          readstate.LastMessageId != storageChannel.raw.LastMessageId;
                        }
                        if (LocalState.CurrentGuild.channels[sc.Id].permissions.ReadMessages || App.CurrentGuildId == sc.Id)
                        {
                            returnChannels.Add(sc);
                        }
                        break;
                    case 2:
                        if (LocalState.CurrentGuild.channels[sc.Id].permissions.Connect || App.CurrentGuildId == sc.Id)
                        {
                            returnChannels.Add(sc);
                        }
                        break;
                    case 4:
                        if (LocalState.CurrentGuild.channels[sc.Id].permissions.ReadMessages || LocalState.CurrentGuild.channels[sc.Id].permissions.Connect)
                        {
                            sc.Name = sc.Name.ToUpper();
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

        public static List<SimpleChannel> OrderChannels(List<DirectMessageChannel> channels)
        {
            List<SimpleChannel> returnChannels = new List<SimpleChannel>();
            foreach (var channel in channels)
            {
                SimpleChannel sc = new SimpleChannel();
                sc.Id = channel.Id;
                sc.Type = channel.Type;
                switch (channel.Type)
                {
                    case 1: //DM
                        sc.Name = "@" + channel.Users.FirstOrDefault().Username;
                        sc.UserId = channel.Users.FirstOrDefault().Id;
                        sc.LastMessageId = channel.LastMessageId;
                        sc.ImageURL = "https://cdn.discordapp.com/avatars/" + channel.Users.FirstOrDefault().Id + "/" + channel.Users.FirstOrDefault().Avatar + ".png?size=64";

                        sc.Members = new Dictionary<string, User>();
                        foreach (User user in channel.Users)
                        {
                            sc.Members.Add(user.Id, user);
                        }

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
                        returnChannels.Add(sc);
                        break;
                    case 3: //Group
                        sc.Name = channel.Name;
                        sc.LastMessageId = channel.LastMessageId;
                        sc.Subtitle = App.GetString("/Main/members").Replace("<count>", (channel.Users.Count() + 1).ToString());
                        sc.Icon = channel.Icon;
                        //sc.Members = new Dictionary<string, User>();
                        //foreach (User user in channel.Users)
                        //{
                        //    sc.Members.Add(user.Id, user);
                        //}

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
                        returnChannels.Add(sc);
                        break;
                }
            }

            
            return returnChannels.OrderByDescending(x => x.LastMessageId).ToList();
        }
    }
}
