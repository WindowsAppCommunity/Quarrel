using System;
using System.Collections;
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
using Windows.ApplicationModel.Contacts;

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
                set { if (_user == value) return; _user = value; OnPropertyChanged("User"); }
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
            if (!App.Insider)
            {
                ActivitiesLVI.Visibility = Visibility.Collapsed;
                ActivitiesPI.Visibility = Visibility.Collapsed;
                pivot.SelectedIndex = 1;
            }
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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    SimpleFriend sfriend = await NewSF(e.EventData);
                    if (sfriend.RelationshipStatus == 1)
                        AllView.Items.Add(sfriend);
                    else if (sfriend.RelationshipStatus == 2)
                        BlockedView.Items.Add(sfriend);
                    else if (sfriend.RelationshipStatus == 3 || sfriend.RelationshipStatus == 4)
                    {
                        if (sfriend.RelationshipStatus == 3)
                            FriendNotification(sfriend.User);
                        App.FriendNotifications += 1;
                        App.UpdateUnreadIndicators();
                        PendingCounter.Text = App.FriendNotifications.ToString();
                        PendingView.Items.Add(NewSF(e.EventData));
                    }
                });
        }
        private async Task<SimpleFriend> NewSF(KeyValuePair<string, Friend> f)
        {
            return await NewSF(f.Value);
        }
        private async Task<SimpleFriend> NewSF(Friend f)
        {
            var friend = new SimpleFriend();
            friend.User = f.user;
            friend.RelationshipStatus = f.Type;
            friend.SharedGuilds = new List<SimpleFriend.SharedGuild>();
            //TODO: real fix instead of work around.
            if (f.Type != 2)
            {
                UserProfile profile = await RESTCalls.GetUserProfile(friend.User.Id);
                foreach (MutualGuild guild in profile.MutualGuilds)
                {
                    friend.SharedGuilds.Add(new SimpleFriend.SharedGuild()
                    {
                        Id = guild.Id,
                        ImageUrl = LocalState.Guilds[guild.Id].Raw.Icon,
                        Name = LocalState.Guilds[guild.Id].Raw.Name
                    });

                }
            }
          /*  foreach (var guild in LocalState.Guilds)
            {
                if (guild.Value.members.ContainsKey(friend.User.Id))
                    friend.SharedGuilds.Add(new SimpleFriend.SharedGuild()
                    {
                        Id = guild.Value.Raw.Id,
                        ImageUrl = guild.Value.Raw.Icon,
                        Name = guild.Value.Raw.Name
                    });
            }*/
            if (LocalState.PresenceDict.ContainsKey(f.user.Id))
                friend.UserStatus = LocalState.PresenceDict[f.user.Id].FirstOrDefault().Value.Status;
            else
                friend.UserStatus = "offline";
            return friend;
        }

        public async void Load()
        {
            AllView.Items.Clear();
            PendingView.Items.Clear();
            BlockedView.Items.Clear();
            int pending = 0;
            var contactManager = new Managers.ContactManager();
            foreach (var f in LocalState.Friends.ToList())
            {
                try
                {
                    var friend = await NewSF(f);
                    if (f.Value.Type == 3 || f.Value.Type == 4)
                    {
                        pending++;
                        PendingView.Items.Add(friend);
                    }
                    else if (friend.RelationshipStatus == 1)
                    {
                        AllView.Items.Add(friend);
                        try
                        {
                            await contactManager.AddContact(f.Value.user);
                        }

                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception.Message);
                        }
                    }

                    else if (friend.RelationshipStatus == 2)
                        BlockedView.Items.Add(friend);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            PendingCounter.Text = pending.ToString();
            App.FriendNotifications = pending;
            GatewayManager.Gateway.RelationShipAdded += Gateway_RelationShipAdded;
            GatewayManager.Gateway.RelationShipRemoved += Gateway_RelationShipRemoved;
            GatewayManager.Gateway.RelationShipUpdated += Gateway_RelationShipUpdated;
            LoadFeed();
        }

        public bool ContainsString(string[] items, string search)
        {
            foreach (string item in items)
                if (item == search) return true;
            return false;
        }

        public async void LoadFeed()
        {
            //Get feed settings
            LocalState.FeedSettings = await RESTCalls.GetFeedSettings();
            var activities = await RESTCalls.GetActivites();
            string GameIDs;
            List<string> relevantIds = new List<string>();
            List<ActivityData> relevantActivities = new List<ActivityData>();
            foreach(var activity in activities)
            {
                if (!ContainsString(LocalState.FeedSettings.UnsubscribedUsers, activity.UserId) || !ContainsString(LocalState.FeedSettings.UnsubscribedGames, activity.ApplicationId))
                {
                    relevantIds.Add(activity.ApplicationId);
                    relevantActivities.Add(activity);
                }
            }
            var gamenews = await RESTCalls.GetGameNews(relevantIds.ToArray());
            Dictionary<string, GameNews> heroNews = new Dictionary<string, GameNews>();
            if (gamenews != null)
            {
                var gnCount = gamenews.Count();

                foreach (var news in gamenews)
                {
                    //The GameNews list is ordered by game and then by timestamp, so the hero feed must be the last news of every game in the list
                    if (heroNews.ContainsKey(news.GameId))
                        heroNews[news.GameId] = news;
                    else
                        heroNews.Add(news.GameId, news);
                    if (!LocalState.GameNews.ContainsKey(news.GameId))
                    {
                        LocalState.GameNews.Add(news.GameId, new List<GameNews>());
                        LocalState.GameNews[news.GameId].Add(news);
                    }
                    else
                    {
                        LocalState.GameNews[news.GameId].Add(news);
                    }
                }
            }

            var heroNewsList = new List<GameNews>();
            foreach (var value in heroNews)
                heroNewsList.Add(value.Value);
            heroNewsList.Sort((x, y) => DateTimeOffset.Compare(y.Timestamp, x.Timestamp));
            for (var i = 0; i < Math.Min(heroNewsList.Count, 4); i++)
            {
                if (LocalState.SupportedGames.ContainsKey(heroNewsList[i].GameId))
                    heroNewsList[i].GameTitle = LocalState.SupportedGames[heroNewsList[i].GameId].Name.ToUpper();
                HeroFeed.Items.Insert(0, heroNewsList[i]);
            }
                
            if (HeroFeed.Items.Count == 0)
            {
                HeroFeed.Visibility = Visibility.Collapsed;
            }
            else
            {
                HeroFeed.SelectedIndex = 0;
            }

            Feed.ItemsSource = relevantActivities;
        }
        public void NavigateToFriendRequests()
        {
            pivot.SelectedIndex = 1;
        }
        public void FriendNotification(User user)
        {
            string toastTitle = user.Username + " " + App.GetString("/Main/Notifications_SentAfriendRequest");
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

        public void Dispose()
        {
            GatewayManager.Gateway.RelationShipAdded -= Gateway_RelationShipAdded;
            GatewayManager.Gateway.RelationShipRemoved -= Gateway_RelationShipRemoved;
            GatewayManager.Gateway.RelationShipUpdated -= Gateway_RelationShipUpdated;
        }
    }
}
