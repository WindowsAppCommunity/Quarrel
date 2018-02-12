using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Controls
{
    public sealed partial class VoiceMemberControl : UserControl
    {
        public class SimpleMember : INotifyPropertyChanged
        {
            private VoiceState _member;
            public VoiceState Member
            {
                get { return _member; }
                set { _member = value; OnPropertyChanged("Member"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        }

        private GuildMember member;

        public VoiceState voiceState;

        public string DisplayedUserId
        {
            get { return (string)GetValue(DisplayedUserIdProperty); }
            set { SetValue(DisplayedUserIdProperty, value); }
        }
        public static readonly DependencyProperty DisplayedUserIdProperty = DependencyProperty.Register(
            nameof(DisplayedUserId),
            typeof(string),
            typeof(VoiceMemberControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VoiceMemberControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedUserIdProperty)
            {
                member = LocalState.Guilds[App.CurrentGuildId].members[DisplayedUserId];
                voiceState = LocalState.VoiceDict[DisplayedUserId];

                username.Text = member.User.Username;

                AvatarBrush.ImageSource = new BitmapImage(Common.AvatarUri(member.User.Avatar, member.User.Id));

                if (member.User.Avatar == null)
                    AvatarBG.Fill = Common.DiscriminatorColor(member.User.Discriminator);
                else
                    AvatarBG.Fill = Common.GetSolidColorBrush("#00000000");

                
                //if (DisplayedFriend.UserStatus != null)
                //    status.Fill = (SolidColorBrush)App.Current.Resources[DisplayedFriend.UserStatus];
                //if (!Session.Online)
                //{
                //    status.Visibility = Visibility.Collapsed;
                //}

                if (voiceState.SelfDeaf || voiceState.ServerDeaf)
                {
                    if (voiceState.ServerDeaf)
                    {
                        //TODO: Change color?
                    } else
                    {
                        //TODO: Change color back?
                    }
                    Deaf.Visibility = Visibility.Visible;
                }
                else
                {
                    Deaf.Visibility = Visibility.Collapsed;
                }

                if (voiceState.SelfMute || voiceState.ServerMute)
                {
                    if (voiceState.ServerMute)
                    {
                        //TODO: Change color?
                    }
                    else
                    {
                        //TODO: Change color back?
                    }
                    Mute.Visibility = Visibility.Visible;
                }
                else
                {
                    Mute.Visibility = Visibility.Collapsed;
                }

                //discriminator.Text = "#" + DisplayedFriend.User.Discriminator;

            }
        }

        public VoiceMemberControl()
        {
            this.InitializeComponent();
            Tapped += OpenMemberFlyout;
        }

        private void OpenMemberFlyout(object sender, TappedRoutedEventArgs e)
        {
            App.ShowMemberFlyout(this, member.User);
        }
    }
}
