using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class FriendControl : UserControl
    {
        public FriendPanel.SimpleFriend DisplayedFriend
        {
            get { try { return (FriendPanel.SimpleFriend)GetValue(DisplayedFriendProperty); } catch { return null;  } }
            set { SetValue(DisplayedFriendProperty, value); }
        }
        public static readonly DependencyProperty DisplayedFriendProperty = DependencyProperty.Register(
            nameof(DisplayedFriend),
            typeof(FriendPanel.SimpleFriend),
            typeof(FriendControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        public event EventHandler AcceptFriend;
        public event EventHandler RemovedFriend;
        public event EventHandler StartVoiceCall;
        public event EventHandler SentMessage;

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as FriendControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedFriendProperty)
            {
                if (DisplayedFriend == null) return;
                username.Text = DisplayedFriend.User.Username;
                discriminator.Text = "#" + DisplayedFriend.User.Discriminator;
                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(DisplayedFriend.User.Avatar, DisplayedFriend.User.Id));
                
                SharedGuildContainer.Children.Clear();
                if(DisplayedFriend.SharedGuilds != null)
                foreach (var guild in DisplayedFriend.SharedGuilds)
                {
                    Button b = new Button()
                    {
                        Padding = new Thickness(0),
                        Background = new SolidColorBrush(Colors.Transparent),
                        Margin = new Thickness(2,0,2,0),
                        Content = new Rectangle()
                        {
                            Height=36,
                            Width=36,
                            RadiusX=36,
                            RadiusY=36,
                            Fill=new ImageBrush()
                            {
                                ImageSource = new BitmapImage(new Uri(guild.ImageUrl))
                            }
                        },
                        Tag = guild.Id
                    };
                        ToolTipService.SetToolTip(b, guild.Name);
                    b.Click += ClickedGuild;
                    SharedGuildContainer.Children.Add(b);
                }
                if(DisplayedFriend.UserStatus != null)
                status.Fill = (SolidColorBrush)App.Current.Resources[DisplayedFriend.UserStatus];
                if (!Session.Online)
                {
                    status.Visibility = Visibility.Collapsed;
                }
                switch (DisplayedFriend.RelationshipStatus)
                {
                    case 1: //Friend
                        RemoveButton.Visibility = Visibility.Collapsed;
                        AcceptButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Visibility = Visibility.Collapsed;
                        moreButton.Visibility = Visibility.Visible;
                        break;
                    case 2: //Blocked
                        RemoveButton.Visibility = Visibility.Visible;
                        AcceptButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Visibility = Visibility.Collapsed;
                        moreButton.Visibility = Visibility.Collapsed;
                        break;
                    case 3: //Incoming request
                        AcceptButton.Visibility = Visibility.Visible;
                        RemoveButton.Visibility = Visibility.Visible;
                        RelationshipStatus.Visibility = Visibility.Visible;
                        moreButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Text = App.GetString("/Controls/AcceptFriendRequestQ");
                        break;
                    case 4: //Outgoing request
                        AcceptButton.Visibility = Visibility.Collapsed;
                        RemoveButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Visibility = Visibility.Visible;
                        moreButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Text = App.GetString("/Controls/FriendRequestSent");
                        break;
                }
            }
        }

        private void ClickedGuild(object sender, RoutedEventArgs e)
        {
            App.NavigateToGuild((sender as Button).Tag as string);
        }

        public FriendControl()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
                Session.SendFriendRequest(DisplayedFriend.User.Id);

            AcceptFriend?.Invoke(null,null);
        }

        private async void RemoveRelationship(object sender, RoutedEventArgs e)
        {

                Session.RemoveFriend(DisplayedFriend.User.Id);
      
            RemovedFriend?.Invoke(null,null);
        }

        private void moreButton_Click(object sender, RoutedEventArgs e)
        {
            moreButton.Flyout.ShowAt(moreButton);
        }
    }
}
