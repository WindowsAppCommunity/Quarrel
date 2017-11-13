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
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Microsoft.QueryStringDotNET;

using Discord_UWP.Managers;
using Discord_UWP.LocalModels;

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

        private async void Gateway_RelationShipUpdated(object sender, Gateway.GatewayEventArgs<Friend> e)
        {
            await RemoveRelationshipFromUI(e);
            await AddRelationshipToUI(e);
        }
        private async void Gateway_RelationShipRemoved(object sender, Gateway.GatewayEventArgs<Friend> e)
        {
            await RemoveRelationshipFromUI(e);
        }
        private async Task RemoveRelationshipFromUI(Gateway.GatewayEventArgs<Friend> e)
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
            try
            {
                await RemoveRelationshipFromUI(e);
            }
            catch (Exception exception)
            {
                App.NavigateToBugReport(exception);
            }

            await AddRelationshipToUI(e);
        }
        private async Task AddRelationshipToUI(Gateway.GatewayEventArgs<Friend> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    SimpleFriend sfriend = NewSF(e.EventData);
                    if (sfriend.RelationshipStatus == 1)
                        AllView.Items.Add(sfriend);
                    else if (sfriend.RelationshipStatus == 2)
                        BlockedView.Items.Add(sfriend);
                    else if (sfriend.RelationshipStatus == 3 || sfriend.RelationshipStatus == 4)
                    {
                        if (sfriend.RelationshipStatus == 3)
                            await FriendNotification(sfriend.User);
                        App.FriendNotifications += 1;
                        App.UpdateUnreadIndicators();
                        PendingCounter.Text = App.FriendNotifications.ToString();
                        PendingView.Items.Add(NewSF(e.EventData));
                    }
                    Debug.WriteLine(sfriend);
                });
        }
        private SimpleFriend NewSF(KeyValuePair<string, Friend> f)
        {
            return NewSF(f.Value);
        }
        private SimpleFriend NewSF(Friend f)
        {
            var friend = new SimpleFriend();
            friend.User = f.user;
            friend.RelationshipStatus = f.Type;
            friend.SharedGuilds = new List<SimpleFriend.SharedGuild>();
            foreach (var guild in LocalState.Guilds)
            {
                if (guild.Value.members.ContainsKey(friend.User.Id))
                    friend.SharedGuilds.Add(new SimpleFriend.SharedGuild()
                    {
                        Id = guild.Value.Raw.Id,
                        ImageUrl = "https://discordapp.com/api/guilds/" + guild.Value.Raw.Id + "/icons/" + guild.Value.Raw.Icon + ".jpg",
                        Name = guild.Value.Raw.Name
                    });
            }
            if (LocalState.PresenceDict.ContainsKey(f.user.Id))
                friend.UserStatus = LocalState.PresenceDict[f.user.Id].Status;
            else
                friend.UserStatus = "offline";
            return friend;
        }
        public void Load()
        {
            AllView.Items.Clear();
            PendingView.Items.Clear();
            BlockedView.Items.Clear();
            int pending = 0;
            foreach (var f in LocalState.Friends)
            {
                var friend = NewSF(f);
                if (f.Value.Type == 3 || f.Value.Type == 4)
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
            GatewayManager.Gateway.RelationShipAdded += Gateway_RelationShipAdded;
            GatewayManager.Gateway.RelationShipRemoved += Gateway_RelationShipRemoved;
            GatewayManager.Gateway.RelationShipUpdated += Gateway_RelationShipUpdated;
        }

        public async Task FriendNotification(User user)
        {
            string toastTitle = user.Username + " " + App.GetString("/Main/Notifications_SentAfriendRequest");
            //string imageurl = "http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-01-71-81-permanent/2727.happycanyon1_5B00_1_5D00_.jpg";
            string userPhoto = "https://cdn.discordapp.com/avatars/" + user.Id + "/" +
                               user.Avatar + ".jpg";
            // Construct the visuals of the toast
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                        {
                            new AdaptiveText()
                            {
                                Text = toastTitle,
                                HintMaxLines = 6
                            }
                            /*new AdaptiveImage()
                            {
                                Source = imageurl
                            }*/
                        },
                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = userPhoto,
                        HintCrop = ToastGenericAppLogoCrop.Circle
                    }
                }
            };
            // Construct the actions for the toast (inputs and buttons)


            ToastActionsCustom actions = new ToastActionsCustom()
            {
                Buttons =
                {
                    new ToastButton(App.GetString("/Main/Notifications_Accept"), new QueryString()
                    {
                        { "action", "AddRelationship" },
                        { "id", user.Id }
                    }.ToString())
                    {
                        ActivationType = ToastActivationType.Foreground,
                    },
                    new ToastButton(App.GetString("/Main/Notifications_Refuse"), new QueryString()
                    {
                        { "action", "RemoveRelationship" },
                        { "id", user.Id }
                    }.ToString())
                    {
                        ActivationType = ToastActivationType.Foreground,
                    },
                }
            };
            
            // Now we can construct the final toast content
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions,
                // Arguments when the user taps body of toast
                Launch = new QueryString()
                    {
                        { "action", "Navigate" },
                        { "page", "Friends" }
                    }.ToString()
            };
            // And create the toast notification
            ToastNotification notification = new ToastNotification(toastContent.GetXml());
            // And then send the toast
            ToastNotificationManager.CreateToastNotifier().Show(notification);

        }
    }
}
