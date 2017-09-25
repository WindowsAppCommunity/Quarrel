using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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

        public string ShareText
        {
            get { return (string) GetValue(ShareTextProperty); }
            set { SetValue(ShareTextProperty, value);}
        }
        public static readonly DependencyProperty ShareTextProperty = DependencyProperty.Register(
            nameof(ShareText),
            typeof(string),
            typeof(InviteControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        public event EventHandler DeleteInvite;

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
                InviteCode.Text = DisplayedInvite.String;

                string useField = DisplayedInvite.Uses.ToString();
                if (DisplayedInvite.MaxUses != 0) useField += "/" + DisplayedInvite.MaxUses;
                if (DisplayedInvite.Uses == 1)
                    useField += " " + "use,"; //App.GetString("/Controls/UseSingular") + ", ";
                else
                    useField += " " + "uses,"; //App.GetString("/Controls/UsePlural") + ", ";

                if (DisplayedInvite.Temporary)
                {
                    TempInvite.Visibility = Visibility.Visible;
                    ToolTipService.SetToolTip(TempInvite, "");//App.GetString("InviteTempDesc"));
                }
                else
                {
                    TempInvite.Visibility = Visibility.Collapsed;
                }
                if (DisplayedInvite.MaxAge != 0)
                {
                    var timeDiff = TimeSpan.FromSeconds(DisplayedInvite.MaxAge -
                                                        DateTime.Now.Subtract(creationTime).TotalSeconds);
                    //useField += App.GetString("/Controls/expiresIn") + " " + timeDiff.ToString(@"hh\:mm\:ss");
                    useField += "Expires in" + " " + timeDiff.ToString(@"hh\:mm\:ss");

                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Start();
                    timer.Tick += (sender, o) =>
                    {
                        var timeDiffLive =
                            TimeSpan.FromSeconds(DisplayedInvite.MaxAge -
                                                 DateTime.Now.Subtract(creationTime).TotalSeconds);
                        if (timeDiffLive.TotalSeconds == 0)
                            Description.Text = "Expired Link"; //App.GetString("/Controls/ExpiredLink");
                        else
                            Description.Text = useField.Remove(useField.Length - 8) +
                                               timeDiffLive.ToString(@"hh\:mm\:ss");
                        timer.Start();
                    };
                }
                else
                    useField += "Never Expires"; //App.GetString("/Controls/expiresNever");

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

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteInvite?.Invoke(this.DataContext,null);
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText("http://discord.gg/" + InviteCode.Text);
            Clipboard.SetContent(dp);
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (manager, args) =>
            {
                args.Request.Data.SetWebLink(new Uri("http://discord.gg/" + InviteCode.Text));
                if (DisplayedInvite.Channel.Name != null)
                    //args.Request.Data.Properties.Title = App.GetString("/Controls/InviteTo") + " #" + DisplayedInvite.Channel.Name;
                    args.Request.Data.Properties.Title = "Invite to" + " #" + DisplayedInvite.Channel.Name;
                else
                    //args.Request.Data.Properties.Title = App.GetString("/Controls/InviteTo") + " " + DisplayedInvite.Guild.Name;
                    args.Request.Data.Properties.Title = "Invite to" + " " + DisplayedInvite.Guild.Name;
            };
            DataTransferManager.ShowShareUI();
        }
    }
}
