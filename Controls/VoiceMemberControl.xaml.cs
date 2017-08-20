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
using Discord_UWP.CacheModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class VoiceMemberControl : UserControl
    {
        public class SimpleMember : INotifyPropertyChanged
        {
            private Member _member;
            public Member Member
            {
                get { return _member; }
                set { if (_member.Equals(value)) return; _member = value; OnPropertyChanged("Member"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        }

        public SimpleMember DisplayedMember
        {
            get { try { return (SimpleMember)GetValue(DisplayedMemberProperty); } catch { return null;  } }
            set { SetValue(DisplayedMemberProperty, value); }
        }
        public static readonly DependencyProperty DisplayedMemberProperty = DependencyProperty.Register(
            nameof(DisplayedMember),
            typeof(SimpleMember),
            typeof(VoiceMemberControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VoiceMemberControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedMemberProperty)
            {
                if (DisplayedMember == null) return;
                username.Text = DisplayedMember.Member.Raw.User.Username;
                //discriminator.Text = "#" + DisplayedFriend.User.Discriminator;
                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(DisplayedMember.Member.Raw.User.Avatar, DisplayedMember.Member.Raw.User.Id));
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
            App.ShowMemberFlyout(this, DisplayedMember.Member.Raw.User);
        }
    }
}
