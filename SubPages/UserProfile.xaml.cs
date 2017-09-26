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

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;

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
            CommonSrvItem.Visibility = Visibility.Collapsed;
            CommonFrdItem.Visibility = Visibility.Collapsed;
            CommonSrvspivot.Visibility = Visibility.Collapsed;
            CommonFrdspivot.Visibility = Visibility.Collapsed;
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
            profile = await RESTCalls.GetUserProfile(e.Parameter as string); //TODO: Rig to App.Events (maybe, probably not actually)
            if (LocalState.Friends.ContainsKey(profile.User.Id))
            {
                profile.Friend = LocalState.Friends[profile.User.Id];
            }
            else
            {
                profile.Friend = null;
            }

            username.Text = profile.User.Username;
            username.Fade(1, 400);
            discriminator.Text = "#" + profile.User.Discriminator;
            discriminator.Fade(0.4f, 800);

            if (profile.Friend.HasValue)
            {
                SwitchFriendValues(profile.Friend.Value.Type);
            } else if (profile.User.Id == LocalState.CurrentUser.Id)
            {

            } else
            {
                //None
                sendFriendRequest.Visibility = Visibility.Visible;
                SendMessageLink.Visibility = Visibility.Visible;
                Block.Visibility = Visibility.Visible;
            }


            if (LocalState.Notes.ContainsKey(profile.User.Id))
                NoteBox.Text = LocalState.Notes[profile.User.Id];

            GatewayManager.Gateway.UserNoteUpdated += Gateway_UserNoteUpdated;
            GatewayManager.Gateway.RelationShipAdded += Gateway_RelationshipAdded;
            GatewayManager.Gateway.RelationShipUpdated += Gateway_RelationshipUpdated;
            GatewayManager.Gateway.RelationShipRemoved += Gateway_RelationshipRemoved;

            BackgroundGrid.Blur(8,0).Start();

            try
            {

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
                        element.Name = LocalState.Guilds[element.Id].Raw.Name;
                        element.ImagePath = "https://discordapp.com/api/guilds/" + LocalState.Guilds[element.Id].Raw.Id + "/icons/" + LocalState.Guilds[element.Id].Raw.Icon + ".jpg";

                        if (element.Nick != null) element.NickVisibility = Visibility.Visible;
                        else element.NickVisibility = Visibility.Collapsed;

                        MutualGuilds.Items.Add(element);
                        NoCommonServers.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch
            {

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
                    ToolTipService.SetToolTip(img,App.GetString("/Dialogs/FlagDiscordStaff").ToUpper());
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
                    ToolTipService.SetToolTip(img, App.GetString("/Dialogs/FlagDiscordPartner").ToUpper());
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
                    ToolTipService.SetToolTip(img, App.GetString("/Dialogs/FlagHypesquad"));
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
                ToolTipService.SetToolTip(img, App.GetString("/Main/PremiumMemberSince") + " " + Common.HumanizeDate(profile.PremiumSince.Value,null));
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
        private async void Gateway_UserNoteUpdated(object sender, GatewayEventArgs<Gateway.DownstreamEvents.UserNote> e)
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
            GatewayManager.Gateway.UserNoteUpdated -= Gateway_UserNoteUpdated;
            GatewayManager.Gateway.RelationShipAdded -= Gateway_RelationshipAdded;
            GatewayManager.Gateway.RelationShipUpdated -= Gateway_RelationshipUpdated;
            GatewayManager.Gateway.RelationShipRemoved -= Gateway_RelationshipRemoved;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private async void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            await RESTCalls.AddNote(profile.User.Id, NoteBox.Text); //TODO: Rig to App.Events
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
                var relationships = await RESTCalls.GetUserRelationShips(profile.User.Id); //TODO: Rig to App.Events (maybe, probably not actually)
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
            await Task.Run(async () =>
            {
                await RESTCalls.SendFriendRequest(profile.User.Id); //TODO: Rig to App.Events
            });

        }

        private async void RemoveFriend(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                await RESTCalls.RemoveFriend(profile.User.Id); //TODO: Rig to App.Events
            });
        }

        private void SendMessageLink_Click(object sender, RoutedEventArgs e)
        {
            App.NavigateToDMChannel(null, profile.User.Id); //TODO: Allow userId DM navigation
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
