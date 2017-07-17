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
        public class SimpleGuild : INotifyPropertyChanged
        {
            /* This is a really ugly class, but it's necessary to have the values update correctly */

            //id = channel id
            //name = the displayed name of the channel
            //imageurl = the URL of the image displayed for DMs
            //notificationcount = the amount of pending notifications in this channel
            //isunread = is the unread messages indicator visible?
            //ismuted = is the channel muted?
            //isdm = if it is the DM control

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

            private string _imageurl;
            public string ImageURL
            {
                get { return _imageurl; }
                set { if (_imageurl == value) return; _imageurl = value; OnPropertyChanged("ImageURL"); }
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

            private bool _ismuted;
            public bool IsMuted
            {
                get { return _ismuted; }
                set { if (_ismuted == value) return; _ismuted = value; OnPropertyChanged("IsMuted"); }
            }

            private bool _isdm;
            public bool IsDM
            {
                get { return _isdm; }
                set { if (_isdm == value) return; _isdm = value; OnPropertyChanged("IsDM"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        }

        private IEnumerable<KeyValuePair<string, Guild>> DisplayedGuilds = null;

        private void LoadGuildList()
        {
            SimpleGuild DM = new SimpleGuild();
            DM.Id = "DMs";
            DM.Name = "Direct Messages";
            DM.IsDM = true;
            foreach (var chn in Storage.Cache.DMs.Values)
                if (Session.RPC.ContainsKey(chn.Raw.Id))
                {
                    ReadState readstate = Session.RPC[chn.Raw.Id];
                    DM.NotificationCount += readstate.MentionCount;
                    var StorageChannel = Storage.Cache.DMs[chn.Raw.Id];
                    if (StorageChannel != null && StorageChannel.Raw.LastMessageId != null &&
                        readstate.LastMessageId != StorageChannel.Raw.LastMessageId)
                        DM.IsUnread = true;
                }
            ServerList.Items.Add(DM);

            foreach (var guild in Storage.Cache.Guilds.OrderBy(x => x.Value.Postition))
            {
                var sg = new SimpleGuild();
                sg.Id = guild.Value.RawGuild.Id;
                if (guild.Value.RawGuild.Icon != null && guild.Value.RawGuild.Icon != "")
                {
                    sg.ImageURL = "https://discordapp.com/api/guilds/" + guild.Value.RawGuild.Id + "/icons/" + guild.Value.RawGuild.Icon + ".jpg";
                } else
                {
                    sg.ImageURL = "empty";
                }
                sg.Name = guild.Value.RawGuild.Name;

                sg.IsMuted = Storage.MutedServers.Contains(guild.Key);
                sg.IsUnread = false; //Will change if true
                foreach (var chn in guild.Value.Channels.Values)
                    if (Session.RPC.ContainsKey(chn.Raw.Id))
                    {
                        ReadState readstate = Session.RPC[chn.Raw.Id];
                        sg.NotificationCount += readstate.MentionCount;
                        var StorageChannel = Storage.Cache.Guilds[sg.Id].Channels[chn.Raw.Id];
                        if (StorageChannel != null && StorageChannel.Raw.LastMessageId != null &&
                            readstate.LastMessageId != StorageChannel.Raw.LastMessageId && !sg.IsMuted)
                            sg.IsUnread = true;
                    }
                ServerList.Items.Add(sg);
            }
        }
    }
}
