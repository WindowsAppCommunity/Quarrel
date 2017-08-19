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
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

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

        private void Gateway_RelationShipUpdated(object sender, Gateway.GatewayEventArgs<Friend> e)
        {
            //TODO Check what the relationshipstatus transition is, remove from the correct AllView, BlockedView, or PendingView, and modify in All
            Gateway_RelationShipRemoved(null, e);
            Gateway_RelationShipAdded(null, e);
        }

        private async void Gateway_RelationShipRemoved(object sender, Gateway.GatewayEventArgs<Friend> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
              () =>
              {
                  var insideAllView = AllView.Items.FirstOrDefault(x => (x as SimpleFriend).User.Id == e.EventData.Id);
                  if (insideAllView != null)
                      AllView.Items.Remove(insideAllView);

                  var insidePendingView = PendingView.Items.FirstOrDefault(x => (x as SimpleFriend).User.Id == e.EventData.Id);
                  if (insidePendingView != null)
                  {
                      PendingView.Items.Remove(insidePendingView);
                      App.FriendNotifications -= 1;
                      PendingCounter.Text = App.FriendNotifications.ToString();
                  }

                  var insideBlockedView = BlockedView.Items.FirstOrDefault(x => (x as SimpleFriend).User.Id == e.EventData.Id);
                  if (insideBlockedView != null)
                      BlockedView.Items.Remove(insideBlockedView);
              });

        }

        private async void Gateway_RelationShipAdded(object sender, Gateway.GatewayEventArgs<Friend> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    var friend = NewSF(e.EventData);
                    if (friend.RelationshipStatus == 1)
                        AllView.Items.Add(NewSF(e.EventData));
                    else if (friend.RelationshipStatus == 2)
                        BlockedView.Items.Add(NewSF(e.EventData));
                    else if (friend.RelationshipStatus == 3 || friend.RelationshipStatus == 4)
                    {
                        App.FriendNotifications += 1;
                        App.UpdateUnreadIndicators();
                        PendingCounter.Text = App.FriendNotifications.ToString();
                        PendingView.Items.Add(NewSF(e.EventData));
                    }
                });
        }
        private SimpleFriend NewSF(KeyValuePair<string, CacheModels.Friend> f)
        {
            var friend = new SimpleFriend();
            friend.User = f.Value.Raw.user;
            friend.RelationshipStatus = f.Value.Raw.Type;
            friend.SharedGuilds = new List<SimpleFriend.SharedGuild>();
            foreach (var guild in Storage.Cache.Guilds)
            {
                if (guild.Value.Members.ContainsKey(friend.User.Id))
                    friend.SharedGuilds.Add(new SimpleFriend.SharedGuild()
                    {
                        Id = guild.Value.RawGuild.Id,
                        ImageUrl = "https://discordapp.com/api/guilds/" + guild.Value.RawGuild.Id + "/icons/" + guild.Value.RawGuild.Icon + ".jpg",
                        Name = guild.Value.RawGuild.Name
                    });
            }
            return friend;
        }
        private SimpleFriend NewSF(Friend f)
        {
            var friend = new SimpleFriend();
            friend.User = f.user;
            friend.RelationshipStatus = f.Type;
            friend.SharedGuilds = new List<SimpleFriend.SharedGuild>();
            foreach (var guild in Storage.Cache.Guilds)
            {
                if (guild.Value.Members.ContainsKey(friend.User.Id))
                    friend.SharedGuilds.Add(new SimpleFriend.SharedGuild()
                    {
                        Id = guild.Value.RawGuild.Id,
                        ImageUrl = "https://discordapp.com/api/guilds/" + guild.Value.RawGuild.Id + "/icons/" + guild.Value.RawGuild.Icon + ".jpg",
                        Name = guild.Value.RawGuild.Name
                    });
            }
            return friend;
        }
        public async void Load()
        {
            int pending = 0;
            foreach (var f in Storage.Cache.Friends)
            {
                var friend = NewSF(f);
                if (f.Value.Raw.Type == 3 || f.Value.Raw.Type == 4)
                {
                    pending++;
                    PendingView.Items.Add(friend);
                }
                else if (friend.RelationshipStatus == 1)
                    AllView.Items.Add(friend);
                else if (friend.RelationshipStatus == 2)
                    BlockedView.Items.Add(friend);
            }
            PendingCounter.Text = pending.ToString();
            App.FriendNotifications = pending;
            Session.Gateway.RelationShipAdded += Gateway_RelationShipAdded;
            Session.Gateway.RelationShipRemoved += Gateway_RelationShipRemoved;
            Session.Gateway.RelationShipUpdated += Gateway_RelationShipUpdated;
        }
    }
}
