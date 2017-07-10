using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class InviteControl : UserControl
    {
        public Invite DisplayedInvite
        {
            get { return (Invite)GetValue(DisplayedInviteProperty); }
            set { SetValue(DisplayedInviteProperty, value); }
        }
        public static readonly DependencyProperty DisplayedInviteProperty = DependencyProperty.Register(
            nameof(DisplayedInvite),
            typeof(Invite),
            typeof(InviteControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as InviteControl;

            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedInviteProperty)
            {
                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(DisplayedInvite.Inviter.Avatar, DisplayedInvite.Inviter.Id));
                Username.Text = DisplayedInvite.Inviter.Username;
                var creationTime = DateTime.Parse(DisplayedInvite.CreatedAt);
                CreationTime.Text = Common.HumanizeDate(creationTime, null);
                InviteCode.Text = DisplayedInvite.String;

                string useField = DisplayedInvite.Uses.ToString();
                if (DisplayedInvite.MaxUses != 0) useField += "/" + DisplayedInvite.MaxUses;
                if (DisplayedInvite.Uses == 1)
                    useField += "use, ";
                else
                    useField += " uses, ";

                if (!DisplayedInvite.Temporary)
                {
                    useField += "no expiry date";
                }
                else if(DisplayedInvite.MaxAge != 0)
                {
                    var timeDiff = TimeSpan.FromSeconds(DisplayedInvite.MaxAge - DateTime.Now.Subtract(creationTime).TotalSeconds);
                    useField += "expires in " + timeDiff.ToString("hh:mm:ss");

                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Start();
                    timer.Tick += (sender, o) =>
                    {
                        var timeDiffLive = TimeSpan.FromSeconds(DisplayedInvite.MaxAge - DateTime.Now.Subtract(creationTime).TotalSeconds);
                        if (timeDiffLive.TotalSeconds == 0)
                            Description.Text = "Expired link";
                        else
                            Description.Text = useField.Remove(useField.Length - 8) + timeDiffLive.ToString("hh:mm:ss");
                    };
                }
                Description.Text = useField;
            }
        }

        public InviteControl()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
