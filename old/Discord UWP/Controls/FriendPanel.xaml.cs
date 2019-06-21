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
using DiscordAPI.API.Gateway;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Microsoft.QueryStringDotNET;
using Windows.ApplicationModel.Contacts;
using DiscordAPI.API.User.Models;
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.API.Gateway;
using DiscordAPI.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class FriendPanel : UserControl
    {
        public class SimpleFriend : INotifyPropertyChanged
        {
            public class SharedGuild
            {
                /// <summary>
                /// Id of Guild
                /// </summary>
                public string Id { get; set; }

                /// <summary>
                /// Name of Guild
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// Url of Guild Icon
                /// </summary>
                public string ImageUrl { get; set; }
            }

            private User _user;
            /// <summary>
            /// User object of Friend
            /// </summary>
            public User User
            {
                get { return _user; }
                set { if (_user == value) return; _user = value; OnPropertyChanged("User"); }
            }

            private string _userstatus;
            /// <summary>
            /// Presence Online Status of Friend user
            /// </summary>
            public string UserStatus
            {
                get { return _userstatus; }
                set { if (_userstatus == value) return; _userstatus = value; OnPropertyChanged("UserStatus"); }
            }

            private int _relationshipstatus;
            /// <summary>
            /// The relationship status of Friend according to Current User
            /// </summary>
            public int RelationshipStatus
            {
                get { return _relationshipstatus; }
                set { if (_relationshipstatus == value) return; _relationshipstatus = value; OnPropertyChanged("RelationshipStatus"); }
            }

            public List<SharedGuild> _sharedGuilds;
            /// <summary>
            /// List of Guilds both Friend User and Current User are part of
            /// </summary>
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
                // Hide Activies Panel because it's broken
                ActivitiesLVI.Visibility = Visibility.Collapsed;
                ActivitiesPI.Visibility = Visibility.Collapsed;
                pivot.SelectedIndex = 1;
            }
        }

        /// <summary>
        /// Update Relationship when changed
        /// </summary>
        private async void Gateway_RelationShipUpdated(object sender, GatewayEventArgs<Friend> e)
        {
            await RemoveRelationshipFromUI(e);
            await AddRelationshipToUI(e);
        }

        /// <summary>
        /// Remove Relationship when removed
        /// </summary>
        private async void Gateway_RelationShipRemoved(object sender, GatewayEventArgs<Friend> e)
        {
            await RemoveRelationshipFromUI(e);
        }

        /// <summary>
        /// Remove user from Panel
        /// </summary>
        private async Task RemoveRelationshipFromUI(GatewayEventArgs<Friend> e)
        {
            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    // Item from All View
                    var insideAllView = AllView.Items.FirstOrDefault(x => (x as SimpleFriend).User.Id == e.EventData.Id);
                    if (insideAllView != null)
                        AllView.Items.Remove(insideAllView);

                    // Remove from Pending View
                    var insidePendingView = PendingView.Items.FirstOrDefault(x => (x as SimpleFriend).User.Id == e.EventData.Id);
                    if (insidePendingView != null)
                    {
                        PendingView.Items.Remove(insidePendingView);
                        App.FriendNotifications -= 1;
                        PendingCounter.Text = App.FriendNotifications.ToString();
                    }

                    // Remove from Blocked View
                    var insideBlockedView = BlockedView.Items.FirstOrDefault(x => (x as SimpleFriend).User.Id == e.EventData.Id);
                    if (insideBlockedView != null)
                        BlockedView.Items.Remove(insideBlockedView);
                });
        }

        /// <summary>
        /// Add RelationShip when added
        /// </summary>
        private async void Gateway_RelationShipAdded(object sender, GatewayEventArgs<Friend> e)
        {
            if (e.EventData.Type == 1)
            {
                await RemoveRelationshipFromUI(e);
            }
            await AddRelationshipToUI(e);
        }

        /// <summary>
        /// Add relation ship to Panel
        /// </summary>
        private async Task AddRelationshipToUI(GatewayEventArgs<Friend> e)
        {
            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    // Create SimpleFriend Object
                    SimpleFriend sfriend = await NewSF(e.EventData);

                    // Add to appropiate list(s)
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
                        PendingView.Items.Add(sfriend);
                    }
                });
        }

        /// <summary>
        /// Create SimpleFriend from Dictionary item
        /// </summary>
        /// <param name="f">KeyValuePair</param>
        /// <returns>SimpleFriend</returns>
        private async Task<SimpleFriend> NewSF(KeyValuePair<string, Friend> f)
        {
            return await NewSF(f.Value);
        }

        /// <summary>
        /// Create SimpleFriend from API Friend data
        /// </summary>
        /// <param name="f">API Friend data</param>
        /// <returns>SimpleFriend</returns>
        private async Task<SimpleFriend> NewSF(Friend f)
        {
            // Blank tample
            var friend = new SimpleFriend();

            // Set basic user details
            friend.User = f.user;
            friend.RelationshipStatus = f.Type;

            // Add Shared Guilds to List: "SharedGuilds"
            friend.SharedGuilds = new List<SimpleFriend.SharedGuild>();
            //TODO: real fix instead of work around.
            if (f.Type != 2)
            {
                UserProfile profile = await RESTCalls.GetUserProfile(friend.User.Id);
                if (profile.MutualGuilds != null)
                {
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
            }

            // Set Presence
            if (LocalState.PresenceDict.ContainsKey(f.user.Id))
                friend.UserStatus = LocalState.PresenceDict[f.user.Id].Status;
            else
                friend.UserStatus = "offline";

            // Return value
            return friend;
        }

        /// <summary>
        /// Load Page
        /// </summary>
        public async void Load()
        {
            // Clear old items
            AllView.Items.Clear();
            PendingView.Items.Clear();
            BlockedView.Items.Clear();

            // pending is the number of pending friend requests to display
            int pending = 0;
            
            // Quarrel integrates with People app
            var contactManager = new Managers.ContactManager();

            foreach (var f in LocalState.Friends.ToList())
            {
                try
                {
                    // Create Friend object
                    var friend = await NewSF(f);

                    // Add Friend to appropiate list(s)
                    if (f.Value.Type == 3 || f.Value.Type == 4)
                    {
                        pending++;
                        PendingView.Items.Add(friend);
                    }
                    else if (friend.RelationshipStatus == 1)
                    {
                        // Add friends to list and people app s
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

            // Set UI and global pending values
            PendingCounter.Text = pending.ToString();
            App.FriendNotifications = pending;

            // Add update events
            GatewayManager.Gateway.RelationShipAdded += Gateway_RelationShipAdded;
            GatewayManager.Gateway.RelationShipRemoved += Gateway_RelationShipRemoved;
            GatewayManager.Gateway.RelationShipUpdated += Gateway_RelationShipUpdated;

            // Load Activities
            LoadFeed();
        }

        public bool ContainsString(string[] items, string search)
        {
            foreach (string item in items)
                if (item == search) return true;
            return false;
        }

        /// <summary>
        /// Load Activites panel
        /// </summary>
        public async void LoadFeed()
        {
            //Get feed settings
            LocalState.FeedSettings = await RESTCalls.GetFeedSettings();

            // Get activies
            var activities = await RESTCalls.GetActivites();

            // Id list of Relevant Activites
            List<string> relevantIds = new List<string>();
            // Actives actually in use by friends
            List<ActivityData> relevantActivities = new List<ActivityData>();

            // Determine relevant activites
            foreach(var activity in activities)
            {
                if (!ContainsString(LocalState.FeedSettings.UnsubscribedUsers, activity.UserId) || !ContainsString(LocalState.FeedSettings.UnsubscribedGames, activity.ApplicationId))
                {
                    relevantIds.Add(activity.ApplicationId);
                    relevantActivities.Add(activity);
                }
            }

            // Get news feed
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

            // Hero news list is the FlipView news
            var heroNewsList = new List<GameNews>();
            foreach (var value in heroNews)
                heroNewsList.Add(value.Value);
            heroNewsList.Sort((x, y) => DateTimeOffset.Compare(y.Timestamp, x.Timestamp));

            // Add top 4 more recent news items to the FlipView in reverse order
            for (var i = 0; i < Math.Min(heroNewsList.Count, 4); i++)
            {
                if (LocalState.SupportedGames.ContainsKey(heroNewsList[i].GameId))
                    heroNewsList[i].GameTitle = LocalState.SupportedGames[heroNewsList[i].GameId].Name.ToUpper();
                HeroFeed.Items.Insert(0, heroNewsList[i]);
            }

            // If empty, hide FlipView
            if (HeroFeed.Items.Count == 0)
            {
                HeroFeed.Visibility = Visibility.Collapsed;
            }
            else
            {
                HeroFeed.SelectedIndex = 0;
            }

            // Show activites
            Feed.ItemsSource = relevantActivities;
        }

        /// <summary>
        /// Navigate to Friend Requests panel
        /// </summary>
        public void NavigateToFriendRequests()
        {
            pivot.SelectedIndex = 1;
        }

        /// <summary>
        /// Show Toast of recieving a Friend Request
        /// </summary>
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

        /// <summary>
        /// Send Friend Request Button pressed
        /// </summary>
        private async void SendFriendRequest(object sender, RoutedEventArgs e)
        {
            // Split at Descriminator
            string[] strings = SendFriendTB.Text.Split('#');

            if (strings.Count() == 2)
            {
                // Send Friend Request
                SendFriendRequestResponse result =
                    await RESTCalls.SendFriendRequest(strings[0], Convert.ToInt32(strings[1]));
                if (result != null && result.Message != null)
                    FriendRequestStatus.Text =
                        result.Message; //App.GetString(result.Message.Replace(' ', '\0')); //TODO: Translate
                else
                    FriendRequestStatus.Text = App.GetString("/Controls/Success");
            }
            else
            {
                // Invalid input
                FriendRequestStatus.Text = App.GetString("/Controls/NeedDesc");
            }
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            GatewayManager.Gateway.RelationShipAdded -= Gateway_RelationShipAdded;
            GatewayManager.Gateway.RelationShipRemoved -= Gateway_RelationShipRemoved;
            GatewayManager.Gateway.RelationShipUpdated -= Gateway_RelationShipUpdated;
        }
    }
}
