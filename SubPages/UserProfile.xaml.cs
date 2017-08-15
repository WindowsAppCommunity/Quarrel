using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.Gateway;
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp.UI.Animations;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class UserProfile : Page
    {
        public UserProfile()
        {
            this.InitializeComponent();
            sendFriendRequest.Content = App.Translate("SendFriendRequest");
            acceptFriend.Content = App.Translate("AcceptFriendRequest");
            pendingFriend.Content = App.Translate("PendingFriendRequest");
            Unblock.Content = App.Translate("Unblock");
            DetailsItem.Content = App.Translate("Details").ToUpper();
            CommonSrvItem.Content = App.Translate("CommonServer").ToUpper();
            CommonFrdItem.Content = App.Translate("CommonFriend").ToUpper();
            NoteText.Text = App.Translate("Note").ToUpper();
            if (!Session.Online)
            {
                CommonSrvItem.Visibility = Visibility.Collapsed;
                CommonFrdItem.Visibility = Visibility.Collapsed;
                CommonSrvspivot.Visibility = Visibility.Collapsed;
                CommonFrdspivot.Visibility = Visibility.Collapsed;
            }
        }
        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private SharedModels.UserProfile profile;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!(e.Parameter is string))
            {
                CloseButton_Click(null, null);
                return;
            }
            if (Session.Online)
            {
                profile = await Session.GetUserProfile(e.Parameter as string);
                if (Storage.Cache.Friends.ContainsKey(profile.User.Id))
                {
                    profile.Friend = Storage.Cache.Friends[profile.User.Id].Raw;
                }
                else
                {
                    profile.Friend = null;
                }
            } else
            {
                if (App.CurrentGuildIsDM)
                {
                    foreach (CacheModels.DmCache chn in Storage.Cache.DMs.Values)
                    {
                        if (chn.Raw.Type == 1 && chn.Raw.Users.FirstOrDefault().Id == e.Parameter as string)
                        {
                            profile = new SharedModels.UserProfile()
                            {
                                ConnectedAccount = null,
                                Friend = null,
                                MutualGuilds = null,
                                PremiumSince = null,
                                User = chn.Raw.Users.FirstOrDefault()
                            };
                        }
                    }
                } else
                {
                    if (Storage.Cache.Guilds[App.CurrentGuildId].Members.ContainsKey(e.Parameter as string))
                    {
                        profile = new SharedModels.UserProfile()
                        {
                            ConnectedAccount = null,
                            Friend = null,
                            MutualGuilds = null,
                            PremiumSince = null,
                            User = Storage.Cache.Guilds[App.CurrentGuildId].Members[e.Parameter as string].Raw.User
                        };
                    }
                }

                if (Storage.Cache.Friends.ContainsKey(e.Parameter as string))
                {
                    profile.Friend = Storage.Cache.Friends[e.Parameter as string].Raw;
                }
            }

            username.Text = profile.User.Username;
            username.Fade(1, 400);
            discriminator.Text = "#" + profile.User.Discriminator;
            discriminator.Fade(0.4f, 800);

            if (profile.Friend.HasValue)
            {
                SwitchFriendValues(profile.Friend.Value.Type);
            } else if (profile.User.Id == Storage.Cache.CurrentUser.Raw.Id)
            {

            } else
            {
                //None
                sendFriendRequest.Visibility = Visibility.Visible;
                SendMessageLink.Visibility = Visibility.Visible;
                Block.Visibility = Visibility.Visible;
            }

            if (!Session.Online)
            {
                acceptFriend.Visibility = Visibility.Collapsed;
                Unblock.Visibility = Visibility.Collapsed;
                sendFriendRequest.Visibility = Visibility.Collapsed;
                SendMessageLink.Visibility = Visibility.Collapsed;
                Block.Visibility = Visibility.Collapsed;
                NoteBox.IsReadOnly = true;
            }


            if (App.Notes.ContainsKey(profile.User.Id))
                NoteBox.Text = App.Notes[profile.User.Id];

            if (Session.Online)
            {
                Session.Gateway.UserNoteUpdated += Gateway_UserNoteUpdated;
                Session.Gateway.RelationShipAdded += Gateway_RelationshipAdded;
                Session.Gateway.RelationShipUpdated += Gateway_RelationshipUpdated;
                Session.Gateway.RelationShipRemoved += Gateway_RelationshipRemoved;
            }


            BackgroundGrid.Blur(8,0).Start();

            if (profile.ConnectedAccount != null)
            {
                for (int i = 0; i < profile.ConnectedAccount.Count(); i++)
                {
                    var element = profile.ConnectedAccount.ElementAt(i);
                    string themeExt = "";
                    if (element.Type.ToLower() == "steam")
                    {
                        if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                            themeExt = "_light";
                        else
                            themeExt = "_dark";
                    }
                    element.ImagePath = "/Assets/ConnectionLogos/" + element.Type.ToLower() + themeExt + ".png";
                    Connections.Items.Add(element);
                }
            }
            
            if (profile.MutualGuilds != null)
            {
                for (int i = 0; i < profile.MutualGuilds.Count(); i++)
                {
                    var element = profile.MutualGuilds.ElementAt(i);
                    element.Name = Storage.Cache.Guilds[element.Id].RawGuild.Name;
                    element.ImagePath = "https://discordapp.com/api/guilds/" + Storage.Cache.Guilds[element.Id].RawGuild.Id + "/icons/" + Storage.Cache.Guilds[element.Id].RawGuild.Icon + ".jpg";

                    if (element.Nick != null) element.NickVisibility = Visibility.Visible;
                    else element.NickVisibility = Visibility.Collapsed;

                    MutualGuilds.Items.Add(element);
                    NoCommonServers.Visibility = Visibility.Collapsed;
                }
            }
            

            switch (profile.User.Flags)
            {
                case 1:
                {
                    var img = new Image()
                    {
                        MaxHeight = 28,
                        Source = new BitmapImage(new Uri("ms-appx:///Assets/DiscordBadges/staff.png")),
                        Opacity=0
                    };
                    ToolTipService.SetToolTip(img,App.Translate("DiscordStaff").ToUpper());
                    BadgePanel.Children.Add(img);
                    img.Fade(1).Start();
                    break;
                }
                case 2:
                {
                    var img = new Image()
                    {
                        MaxHeight = 28,
                        Source = new BitmapImage(new Uri("ms-appx:///Assets/DiscordBadges/partner.png")),
                        Opacity = 0
                    };
                    ToolTipService.SetToolTip(img, App.Translate("DiscordPartner").ToUpper());
                    BadgePanel.Children.Add(img);
                    img.Fade(1).Start();
                    break;
                    }
                case 4:
                {
                    var img = new Image()
                    {
                        MaxHeight = 28,
                        Source = new BitmapImage(new Uri("ms-appx:///Assets/DiscordBadges/hypesquad.png")),
                        Opacity = 0
                    };
                    ToolTipService.SetToolTip(img, "HYPESQUAD");
                    BadgePanel.Children.Add(img);
                    img.Fade(1).Start();
                    break;
                }
            }

            if (profile.PremiumSince.HasValue)
            {
                var img = new Image()
                {
                    MaxHeight = 28,
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/DiscordBadges/nitro.png")),
                    Opacity = 0
                };
                ToolTipService.SetToolTip(img, App.Translate("PremiumMemberSince") + " " + Common.HumanizeDate(profile.PremiumSince.Value,null));
                BadgePanel.Children.Add(img);
                img.Fade(1.2f);
            }
            if (profile.User.Avatar != null)
            {
                var AvatarExtension = ".png";
                if (profile.User.Avatar.StartsWith("a_")) AvatarExtension = ".gif";
                var image = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + profile.User.Id + "/" + profile.User.Avatar + AvatarExtension));
                AvatarFull.ImageSource = image;
                AvatarBlurred.Source = image;
            } else
            {
                var image = new BitmapImage(new Uri("ms-appx:///Assets/DiscordIcon.png"));
                AvatarFull.ImageSource = image;
                AvatarBlurred.Source = image;
            }
        }

        private void Gateway_RelationshipUpdated(object sender, GatewayEventArgs<Friend> gatewayEventArgs)
        {
            
            if(gatewayEventArgs.EventData.user.Id == profile.User.Id)
            SwitchFriendValues(gatewayEventArgs.EventData.Type);
        }

        private void Gateway_RelationshipAdded(object sender, GatewayEventArgs<Friend> gatewayEventArgs)
        {
            if (gatewayEventArgs.EventData.user.Id == profile.User.Id)
                SwitchFriendValues(gatewayEventArgs.EventData.Type);
        }

        private void Gateway_RelationshipRemoved(object sender, GatewayEventArgs<Friend> e)
        {
            if (e.EventData.Id == profile.User.Id)
                SwitchFriendValues(0);
        }

        private async void SwitchFriendValues(int type)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                RemoveFriendLink.Visibility = Visibility.Collapsed;
                Block.Visibility = Visibility.Collapsed;
                SendMessageLink.Visibility = Visibility.Visible;
                pendingFriend.Visibility = Visibility.Collapsed;
                acceptFriend.Visibility = Visibility.Collapsed;
                sendFriendRequest.Visibility = Visibility.Collapsed;
                switch (type)
                {
                    case 0:
                        //No relationship
                        Block.Visibility = Visibility.Visible;
                        sendFriendRequest.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        //Friend
                        RemoveFriendLink.Visibility = Visibility.Visible;
                        SendMessageLink.Visibility = Visibility.Visible;
                        Block.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        //Blocked
                        Unblock.Visibility = Visibility.Visible;
                        SendMessageLink.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        //Pending incoming friend request
                        acceptFriend.Visibility = Visibility.Visible;
                        SendMessageLink.Visibility = Visibility.Visible;
                        Block.Visibility = Visibility.Visible;
                        break;
                    case 4:
                        //Pending outgoing friend request
                        pendingFriend.Visibility = Visibility.Visible;
                        SendMessageLink.Visibility = Visibility.Visible;
                        Block.Visibility = Visibility.Visible;
                        break;
                }
            });
        }
        private async void Gateway_UserNoteUpdated(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.UserNote> e)
        {
            if (e.EventData.UserId == profile.User.Id)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        NoteBox.Text = e.EventData.Note;
                    });
            }
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            if (Session.Online)
            {
                Session.Gateway.UserNoteUpdated -= Gateway_UserNoteUpdated;
                Session.Gateway.RelationShipAdded -= Gateway_RelationshipAdded;
                Session.Gateway.RelationShipUpdated -= Gateway_RelationshipUpdated;
                Session.Gateway.RelationShipRemoved -= Gateway_RelationshipRemoved;
            }
            NavAway.Begin();
            App.SubpageClosed();
        }

        private void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Session.AddNote(profile.User.Id, NoteBox.Text);
        }

        private void FadeIn_ImageOpened(object sender, RoutedEventArgs e)
        {
            (sender as Windows.UI.Xaml.Controls.Image).Fade(0.2f).Start();
        }

        private void AvatarFull_OnImageOpened(object sender, RoutedEventArgs e)
        {
            Avatar.Fade(1).Start();
        }

        private async void Connections_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (ConnectedAccount) e.ClickedItem;
            if(item.Id == null) return;
            string url = null;
            switch (item.Type)
            {
                case "steam": url = "https://steamcommunity.com/profiles/" + item.Id;
                    break;
                case "skype": url = "skype:" + item.Id + "?userinfo";
                    break;
                case "reddit": url = "https://www.reddit.com/u/" + item.Name;
                    break;
                case "facebook": url = "https://www.facebook.com/" + item.Id;
                    break;
                case "patreon": url = "https://www.patreon.com/" + item.Id;
                    break;
                case "twitter": url = "https://www.twitter.com/" + item.Name;
                    break;
                case "twitch": url = "https://www.twitch.tv/" + item.Id;
                    break;
                case "youtube": url = "https://www.youtube.com/channel/" + item.Id;
                    break;
                case "leagueoflegends": url = null;
                    break;
                default: url = null;
                    break;

            }
            if(url != null) await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
        }

        private void MutualGuilds_ItemClick(object sender, ItemClickEventArgs e)
        {
            //TODO: Open guild
        }

        private bool LoadedRelationships = false;

        private async void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pivot.SelectedIndex == 2 && !LoadedRelationships)
            {
                LoadedRelationships = true;
                var relationships = await Session.GetUserRelationShips(profile.User.Id);
                int relationshipcount = relationships.Count();
                LoadingMutualFriends.Fade(0, 200).Start();
                if (relationshipcount == 0)
                    NoMutualFriends.Fade(0.2f, 200).Start();
                else
                    for (int i = 0; i < relationshipcount; i++)
                    {
                        var relationship = relationships.ElementAt(i);
                        relationship.Discriminator = "#" + relationship.Discriminator;
                        if (relationship.Avatar != null) relationship.ImagePath = "https://cdn.discordapp.com/avatars/" + relationship.Id + "/" + relationship.Avatar + ".png";
                        else relationship.ImagePath = "ms-appx:///Assets/DiscordIcon.png";

                        MutualFriends.Items.Add(relationship);
                    }
            }
        }

        private async void SendFriendRequest(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                Session.SendFriendRequest(profile.User.Id);
            });

        }

        private async void RemoveFriend(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                Session.RemoveFriend(profile.User.Id);
            });
        }

        private void SendMessageLink_Click(object sender, RoutedEventArgs e)
        {
            App.NavigateToDMChannel(profile.User.Id);
            CloseButton_Click(null,null);
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public BooleanToVisibilityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible);
        }
    }

    public class BooleanToVisibilityConverterInverse : IValueConverter
    {
        public BooleanToVisibilityConverterInverse()
        {
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Collapsed);
        }
    }
}
