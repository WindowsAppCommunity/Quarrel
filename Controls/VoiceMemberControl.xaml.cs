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
        
        public VoiceState DisplayedMember
        {
            get { return (VoiceState)GetValue(DisplayedMemberProperty); }
            set { SetValue(DisplayedMemberProperty, value); }
        }
        public static readonly DependencyProperty DisplayedMemberProperty = DependencyProperty.Register(
            nameof(DisplayedMember),
            typeof(VoiceState),
            typeof(VoiceMemberControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

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
                member = LocalState.Guilds[App.CurrentGuildId].members[DisplayedMember.UserId];
                username.Text = member.User.Username;
                //discriminator.Text = "#" + DisplayedFriend.User.Discriminator;
                //Avatar.ImageSource = new BitmapImage(Common.AvatarUri(DisplayedMember.Raw.User.Avatar, DisplayedMember.Raw.User.Id));
                //if(DisplayedFriend.UserStatus != null)
                //status.Fill = (SolidColorBrush)App.Current.Resources[DisplayedFriend.UserStatus];
                //if (!Session.Online)
                //{
                //    status.Visibility = Visibility.Collapsed;
                //}

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
