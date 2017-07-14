using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Discord_UWP.SharedModels;
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;

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

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        }



        private IEnumerable<KeyValuePair<string, GuildChannel>> DisplayedChannels = null;

        private void LoadChannelList(List<int> ChannelType)
        {
            if (App.CurrentGuildIsDM) return; //Fuck DMs (for the moment)
            {

                var FilteredChannels = Storage.Cache.Guilds[App.CurrentGuildId]
                    .Channels.Where(x => ChannelType.Contains(x.Value.Raw.Type))
                    .OrderBy(x => x.Value.Raw.Position);
                foreach (var channel in FilteredChannels)
                {
                    foreach (Role role in Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.Roles)
                    {
                        if (!Storage.Cache.Guilds[App.CurrentGuildId].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id))
                        {
                            Storage.Cache.Guilds[App.CurrentGuildId].Members.Add(Storage.Cache.CurrentUser.Raw.Id, new CacheModels.Member(Session.GetGuildMember(App.CurrentGuildId, Storage.Cache.CurrentUser.Raw.Id)));
                        }
                        if (Storage.Cache.Guilds[App.CurrentGuildId].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Count() != 0 && Storage.Cache.Guilds[App.CurrentGuildId].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.First().ToString() == role.Id)
                        {
                            Storage.Cache.Guilds[App.CurrentGuildId].Channels[channel.Value.Raw.Id].chnPerms.GetPermissions(role, Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.Roles);
                        }
                        else if (role.Name == "@everyone" && Storage.Cache.Guilds[App.CurrentGuildId].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Count() != 0)
                        {
                            Storage.Cache.Guilds[App.CurrentGuildId].Channels[channel.Value.Raw.Id].chnPerms.GetPermissions(role, Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.Roles);
                        }
                        else
                        {
                            Storage.Cache.Guilds[App.CurrentGuildId].Channels[channel.Value.Raw.Id].chnPerms.GetPermissions(0);
                        }
                    }

                    Storage.Cache.Guilds[App.CurrentGuildId].Channels[channel.Value.Raw.Id].chnPerms.AddOverwrites(channel.Value.Raw.PermissionOverwrites, App.CurrentGuildId);

                    var sc = new SimpleChannel();
                    sc.Name = channel.Value.Raw.Name;
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
                        sc.ImageURL = "https://cdn.discordapp.com/avatars/" + channel.Value.Members.FirstOrDefault().Key + "/" + channel.Value.Members.FirstOrDefault().Value.Raw.User.Avatar + ".png?size=64";
                        sc.Name = channel.Value.Members.FirstOrDefault().Value.Raw.User.Username;
                        //TODO Check the playing text
                    }
                    else if (type == 3)
                    {
                        //GROUP DM
                        sc.Subtitle = channel.Value.Members.Count().ToString() + " members";
                        List<string> channelMembers = new List<string>();
                        foreach (var d in channel.Value.Members.Values)
                            channelMembers.Add(d.Raw.User.Username);
                        sc.Name = string.Join(", ", channelMembers);
                    }
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

                    TextChannels.Items.Add(sc);
                }
            }
        }
    }
}
