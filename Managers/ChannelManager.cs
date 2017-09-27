using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Managers
{
    class ChannelManager
    {
        public static SimpleChannel MakeChannel(LocalModels.GuildChannel channel)
        {
            SimpleChannel sc = new SimpleChannel();
            sc.Id = channel.raw.Id;
            sc.Name = channel.raw.Name;
            sc.Type = channel.raw.Type;
            switch (channel.raw.Type)
            {
                case 0:
                    sc.IsMuted = LocalState.GuildSettings.ContainsKey(channel.raw.GuildId) ? (LocalState.GuildSettings[channel.raw.GuildId].channelOverrides.ContainsKey(channel.raw.Id) ? LocalState.GuildSettings[channel.raw.GuildId].channelOverrides[channel.raw.Id].Muted : false) : false;
                    if (LocalState.RPC.ContainsKey(sc.Id))
                    {
                        ReadState readstate = LocalState.RPC[sc.Id];
                        sc.NotificationCount = readstate.MentionCount;
                        var StorageChannel = LocalState.Guilds[App.CurrentGuildId].channels[sc.Id];
                        if (StorageChannel != null && StorageChannel.raw.LastMessageId != null &&
                            readstate.LastMessageId != StorageChannel.raw.LastMessageId)
                            sc.IsUnread = true;
                        else
                            sc.IsUnread = false;
                    }
                    if (LocalState.Guilds[App.CurrentGuildId].channels[sc.Id].permissions.Administrator || LocalState.Guilds[App.CurrentGuildId].channels[sc.Id].permissions.ReadMessages || App.CurrentGuildId == sc.Id || LocalState.CurrentUser.Id == LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId)
                    {
                        return sc;
                    }
                    break;
                case 2:
                    //TODO: Voice Channels
                    break;
                case 4:
                    //TODO: Categories
                    break;
            }
            return sc; 
        }

        public class SimpleChannel : INotifyPropertyChanged
        {
            /* This is a really ugly class, but it's necessary to have the values update correctly */

            //id = channel id
            //name = the displayed name of the channel
            //userstatus = the displayed status (online, idle, etc...) as an int
            //subtitle = the text shown below, such as the member count for group DMs or the "Playing __" for DMs
            //imageurl = the URL of the image displayed for DMs
            //type = the type of Channel: 0=text, 1=DM, 2=Voice, 3=GroupDM
            //notificationcount = the amount of pending notifications in this channel
            //isunread = is the unread messages indicator visible?
            //istyping = is someone typing in that channel?
            //ismuted = is the channel muted?

            private string _id;
            public string Id
            {
                get { return _id; }
                set { if (_id == value) return; _id = value; OnPropertyChanged("Id"); }
            }

            private string _parentid;
            public string ParentId
            {
                get { return _parentid; }
                set { if (_parentid == value) return; _parentid = value; OnPropertyChanged("ParentId"); }
            }

            private string _name;
            public string Name
            {
                get { return _name; }
                set { if (_name == value) return; _name = value; OnPropertyChanged("Name"); }
            }

            private string _userstatus;
            public string UserStatus
            {
                get { return _userstatus; }
                set { if (_userstatus == value) return; _userstatus = value; OnPropertyChanged("UserStatus"); }
            }

            private string _subtitle;
            public string Subtitle
            {
                get { return _subtitle; }
                set { if (_subtitle == value) return; _subtitle = value; OnPropertyChanged("Subtitle"); }
            }

            private Game? _playing;
            public Game? Playing
            {
                get { return _playing; }
                set { if (_playing.HasValue && value.HasValue && _playing.Value.Name == value.Value.Name) return; _playing = value; OnPropertyChanged("Playing"); }
            }

            private string _imageurl;
            public string ImageURL
            {
                get { return _imageurl; }
                set { if (_imageurl == value) return; _imageurl = value; OnPropertyChanged("ImageURL"); }
            }

            private int _type;
            public int Type
            {
                get { return _type; }
                set { if (_type == value) return; _type = value; OnPropertyChanged("Type"); }
            }

            private int _position;
            public int Position
            {
                get { return _position; }
                set { if (_position == value) return; _position = value; OnPropertyChanged("Position"); }
            }

            private int _notificationcount;
            public int NotificationCount
            {
                get { return _notificationcount; }
                set { if (_notificationcount == value) return; _notificationcount = value; OnPropertyChanged("NotificationCount"); }
            }

            private bool _isunread;
            public bool IsUnread
            {
                get { return _isunread; }
                set { if (_isunread == value) return; _isunread = value; OnPropertyChanged("IsUnread"); }
            }

            private bool _istyping;
            public bool IsTyping
            {
                get { return _istyping; }
                set { if (_istyping == value) return; _istyping = value; OnPropertyChanged("IsTyping"); }
            }

            private bool _ismuted;
            public bool IsMuted
            {
                get { return _ismuted; }
                set { if (_ismuted == value) return; _ismuted = value; OnPropertyChanged("IsMuted"); }
            }

            private bool _nsfw;
            public bool Nsfw
            {
                get { return _nsfw; }
                set { if (_nsfw == value) return; _nsfw = value; OnPropertyChanged("Nsfw"); }
            }

            private bool _hidden;
            public bool Hidden
            {
                get { return _hidden; }
                set { if (_hidden == value) return; _hidden = value; OnPropertyChanged("Hidden"); }
            }

            private List<string> _members;
            public List<string> Members
            {
                get { return _members; }
                set { if (_members == value) return; _members = value; OnPropertyChanged("Members"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
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
                switch (channel.raw.Type)
                {
                    case 0:
                        sc.IsMuted = LocalState.GuildSettings.ContainsKey(channel.raw.GuildId) ? (LocalState.GuildSettings[channel.raw.GuildId].channelOverrides.ContainsKey(channel.raw.Id) ? LocalState.GuildSettings[channel.raw.GuildId].channelOverrides[channel.raw.Id].Muted : false) : false;
                        if (LocalState.RPC.ContainsKey(sc.Id))
                        {
                            ReadState readstate = LocalState.RPC[sc.Id];
                            sc.NotificationCount = readstate.MentionCount;
                            var StorageChannel = LocalState.Guilds[App.CurrentGuildId].channels[sc.Id];
                            if (StorageChannel != null && StorageChannel.raw.LastMessageId != null &&
                                readstate.LastMessageId != StorageChannel.raw.LastMessageId)
                                sc.IsUnread = true;
                            else
                                sc.IsUnread = false;
                        }
                        if (LocalState.Guilds[App.CurrentGuildId].channels[sc.Id].permissions.Administrator || LocalState.Guilds[App.CurrentGuildId].channels[sc.Id].permissions.ReadMessages || App.CurrentGuildId == sc.Id || LocalState.CurrentUser.Id == LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId)
                        {
                            returnChannels.Add(sc);
                        }
                        break;
                    case 2:
                        //TODO: Voice Channels
                        break;
                    case 4:
                        sc.Name = sc.Name.ToUpper();
                        returnChannels.Add(sc);
                        break;
                }
            }

            //TODO: OrderBy, no parentId on top, ThenBy Category and children
            var Categorized = returnChannels.Where(x => x.ParentId != null && x.Type != 4).OrderBy(x => x.Position);
            var Categories = returnChannels.Where(x => x.Type == 4).OrderBy(x => x.Position).ToList();
            List<SimpleChannel> Sorted = new List<SimpleChannel>();
            foreach (var noId in returnChannels.Where(x => x.ParentId == null && x.Type != 4).OrderBy(x => x.Position))
                Sorted.Add(noId);
            foreach(var categ in Categories)
            {
                Sorted.Add(categ);
                foreach (var item in Categorized.Where(x => x.ParentId == categ.Id))
                    Sorted.Add(item);
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
                        sc.ImageURL = "https://cdn.discordapp.com/avatars/" + channel.Users.FirstOrDefault().Id + "/" + channel.Users.FirstOrDefault().Avatar + ".png?size=64";
                        if (LocalState.PresenceDict.ContainsKey(channel.Users.FirstOrDefault().Id))
                        {
                            sc.UserStatus = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Status;
                            sc.Playing = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game;
                            if (LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game.HasValue)
                            {
                                sc.Playing = new Game()
                                {
                                    Name = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game
                                    .Value.Name,
                                    Type = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game.Value.Type,
                                    Url = LocalState.PresenceDict[channel.Users.FirstOrDefault().Id].Game.Value.Url
                                };
                            }
                        }
                        else
                        {
                            sc.UserStatus = "offline";
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

                        sc.Subtitle = (channel.Users.Count() + 1).ToString() + " " + App.GetString("/Main/members");
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

            //TODO: OrderBy, IsUnread on top

            return returnChannels;
        }
    }
}
