using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Discord_UWP.SharedModels;

using Discord_UWP.LocalModels;

namespace Discord_UWP.Managers
{
    class GuildManager
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

            private bool _isvalid;
            public bool IsValid
            {
                get { return _isvalid; }
                set { if (_isvalid == value) return; _isvalid = value; OnPropertyChanged("IsValid"); }
            }

            public SimpleGuild Clone()
            {
                SimpleGuild sg = new SimpleGuild();
                sg.Id = Id;
                sg.ImageURL = ImageURL;
                sg.IsDM = IsDM;
                sg.IsMuted = IsMuted;
                sg.IsUnread = IsUnread;
                sg.Name = Name;
                sg.NotificationCount = NotificationCount;
                sg.IsValid = IsValid;
                return sg;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        }

        public static SimpleGuild CreateGuild(SharedModels.Guild guild)
        {
            var sg = new SimpleGuild()
            {
                Id = guild.Id,
                Name = guild.Name,
                ImageURL = "https://discordapp.com/api/guilds/" + guild.Id + "/icons/" + guild.Icon + ".jpg",
                IsDM = false,
                IsMuted = LocalState.GuildSettings.ContainsKey(guild.Id) ? LocalState.GuildSettings[guild.Id].raw.Muted : false,
                IsUnread = false, //Will Change if true
                IsValid = true //Will change if false
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
    }
}
