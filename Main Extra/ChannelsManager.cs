using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Discord_UWP.SharedModels;
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;
using Member = Discord_UWP.CacheModels.Member;

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
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

            private List<Member> _members;
            public List<Member> Members
            {
                get { return _members; }
                set { if (_members == value) return; _members = value; OnPropertyChanged("Members"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        }

        private IEnumerable<KeyValuePair<string, GuildChannel>> DisplayedChannels = null;

        private void LoadChannelList(List<int> ChannelType)
        {
            if (App.CurrentGuildIsDM)
            {
                var FilteredChannels = Storage.Cache.DMs.Where(x => ChannelType.Contains(x.Value.Raw.Type));
                foreach (var channel in FilteredChannels)
                {
                    var sc = new SimpleChannel();
                    sc.Id = channel.Value.Raw.Id;
                    var type = channel.Value.Raw.Type;
                    sc.Type = type;
                    if (Storage.MutedChannels.Contains(sc.Id))
                        sc.IsMuted = true;
                    else
                        sc.IsMuted = false;

                    if (type == 1)
                    {
                        //DM
                        sc.ImageURL = "https://cdn.discordapp.com/avatars/" + channel.Value.Raw.Users.FirstOrDefault().Id + "/" + channel.Value.Raw.Users.FirstOrDefault().Avatar + ".png?size=64";
                        sc.Name = channel.Value.Raw.Users.FirstOrDefault().Username;
                        if (Session.PrecenseDict.ContainsKey(channel.Value.Raw.Users.FirstOrDefault().Id))
                        {
                            sc.UserStatus = Session.PrecenseDict[channel.Value.Raw.Users.FirstOrDefault().Id].Status;
                            sc.Playing = Session.PrecenseDict[channel.Value.Raw.Users.FirstOrDefault().Id].Game;
                            if (Session.PrecenseDict[channel.Value.Raw.Users.FirstOrDefault().Id].Game.HasValue)
                            {
                                sc.Playing = new Game() { Name = Session.PrecenseDict[channel.Value.Raw.Users.FirstOrDefault().Id].Game
                                    .Value.Name, Type = Session.PrecenseDict[channel.Value.Raw.Users.FirstOrDefault().Id].Game.Value.Type, Url = Session.PrecenseDict[channel.Value.Raw.Users.FirstOrDefault().Id].Game.Value.Url };
                            }
                        } else
                        {
                            sc.UserStatus = "offline";
                        }
                        //TODO Check the playing text
                    }
                    else if (type == 3)
                    {
                        //GROUP DM
                        sc.Subtitle = channel.Value.Raw.Users.Count().ToString() + " " + App.GetString("/Main/members");
                        if (channel.Value.Raw.Name != null && channel.Value.Raw.Name != "")
                        {
                            sc.Name = channel.Value.Raw.Name;
                        } else
                        {
                            List<string> channelMembers = new List<string>();
                            foreach (var d in channel.Value.Raw.Users)
                                channelMembers.Add(d.Username);
                            sc.Name = string.Join(", ", channelMembers);
                        }
                    }
                    if (Session.RPC.ContainsKey(sc.Id))
                    {
                        ReadState readstate = Session.RPC[sc.Id];
                        sc.NotificationCount = readstate.MentionCount;
                        var StorageChannel = Storage.Cache.DMs[sc.Id];
                        if (StorageChannel != null && StorageChannel.Raw.LastMessageId != null &&
                            readstate.LastMessageId != StorageChannel.Raw.LastMessageId)
                            sc.IsUnread = true;
                        else
                            sc.IsUnread = false;
                    }
                    DirectMessageChannels.Items.Add(sc);
                }
            }
            else 
            {
                var FilteredChannels = Storage.Cache.Guilds[App.CurrentGuildId]
                    .Channels.Where(x => ChannelType.Contains(x.Value.Raw.Type))
                    .OrderBy(x => x.Value.Raw.Position);
                foreach (var channel in FilteredChannels)
                {
                    var sc = new SimpleChannel();
                    sc.Name = channel.Value.Raw.Name;
                    sc.Id = channel.Value.Raw.Id;
                    var type = channel.Value.Raw.Type;
                    sc.Type = type;
                    switch (type)
                    {
                        case 0:
                            if (Storage.MutedChannels.Contains(sc.Id))
                                sc.IsMuted = true;
                            else
                                sc.IsMuted = false;

                            if (Session.RPC.ContainsKey(sc.Id))
                            {
                                ReadState readstate = Session.RPC[sc.Id];
                                sc.NotificationCount = readstate.MentionCount;
                                var StorageChannel = Storage.Cache.Guilds[App.CurrentGuildId].Channels[sc.Id];
                                if (StorageChannel != null && StorageChannel.Raw.LastMessageId != null &&
                                    readstate.LastMessageId != StorageChannel.Raw.LastMessageId)
                                    sc.IsUnread = true;
                                else
                                    sc.IsUnread = false;
                            }
                            if (Storage.Cache.Guilds[App.CurrentGuildId].Channels[sc.Id].chnPerms.Perms.Administrator || Storage.Cache.Guilds[App.CurrentGuildId].Channels[sc.Id].chnPerms.Perms.ReadMessages || App.CurrentGuildId == sc.Id || Storage.Cache.CurrentUser.Raw.Id == Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.OwnerId)
                            {
                                TextChannels.Items.Add(sc);
                            }
                            break;
                        case 2:

                            sc.Members = Storage.Cache.Guilds[channel.Value.Raw.GuildId].Members.Values.TakeWhile(m => m.voicestate.ChannelId == channel.Key).ToList();

                            if (Storage.Cache.Guilds[App.CurrentGuildId].Channels[sc.Id].chnPerms.Perms.Administrator || Storage.Cache.Guilds[App.CurrentGuildId].Channels[sc.Id].chnPerms.Perms.Speak || App.CurrentGuildId == sc.Id || Storage.Cache.CurrentUser.Raw.Id == Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.OwnerId)
                            {
                                VoiceChannels.Items.Add(sc);
                            }
                            break;
                    }
                }
            }
        }
    }
}
