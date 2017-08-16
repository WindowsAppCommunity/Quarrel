using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.Gateway.DownstreamEvents;
using Discord_UWP.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class FriendPanel : UserControl
    {

        public class SimpleFriend : INotifyPropertyChanged
        {
            public class SharedGuild
            {
                public string Id { get; set; }
                public string Name { get; set; }
                public string ImageUrl { get; set; }
            }
            private User _user;
            public User User
            {
                get { return _user; }
                set { if (_user.Equals(value)) return; _user = value; OnPropertyChanged("User"); }
            }

            private string _userstatus;
            public string UserStatus
            {
                get { return _userstatus; }
                set { if (_userstatus == value) return; _userstatus = value; OnPropertyChanged("UserStatus"); }
            }

            private int _relationshipstatus;
            public int RelationshipStatus
            {
                get { return _relationshipstatus; }
                set { if (_relationshipstatus == value) return; _relationshipstatus = value; OnPropertyChanged("RelationshipStatus"); }
            }

            public List<SharedGuild> _sharedGuilds;
            public List<SharedGuild> SharedGuilds
            {
                get { return _sharedGuilds; }
                set { if (_sharedGuilds == value) return; _sharedGuilds = value; OnPropertyChanged("SharedGuilds"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        }
        public FriendPanel()
        {
            this.InitializeComponent();
        }

        public async void Load()
        {
            int pending = 0;
            foreach (var f in Storage.Cache.Friends)
            {
                var friend = new SimpleFriend();
                friend.User = f.Value.Raw.user;
                friend.RelationshipStatus = f.Value.Raw.Type;
                friend.SharedGuilds = new List<SimpleFriend.SharedGuild>();
                foreach(var guild in Storage.Cache.Guilds)
                {
                    if (guild.Value.Members.ContainsKey(friend.User.Id))
                        friend.SharedGuilds.Add(new SimpleFriend.SharedGuild() { Id = guild.Value.RawGuild.Id,
                            ImageUrl = "https://discordapp.com/api/guilds/" + guild.Value.RawGuild.Id + "/icons/" + guild.Value.RawGuild.Icon + ".jpg",
                            Name = guild.Value.RawGuild.Name });
                }
                if (f.Value.Raw.Type == 3 || f.Value.Raw.Type == 4)
                {
                    pending++;
                    PendingView.Items.Add(friend);
                }
                else if (f.Value.Raw.Type == 1)
                    AllView.Items.Add(friend);
                else if (f.Value.Raw.Type == 2)
                    BlockedView.Items.Add(friend);
            }
            PendingCounter.Text = pending.ToString();
        }
    }
}
