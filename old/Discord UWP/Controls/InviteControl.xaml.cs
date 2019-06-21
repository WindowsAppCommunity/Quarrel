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
using DiscordAPI.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class InviteControl : UserControl
    {
        /// <summary>
        /// API Data of invite to display
        /// </summary>
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

        /// <summary>
        /// Event invoked when deleting invite
        /// </summary>
        public event EventHandler DeleteInvite;

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as InviteControl;

            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedInviteProperty)
            {
                // Update user details
                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(DisplayedInvite.Inviter.Avatar, DisplayedInvite.Inviter.Id));
                Username.Text = DisplayedInvite.Inviter.Username;

                // Update invite code
                InviteCode.Text = DisplayedInvite.String;

                // Updated uses
                string useField = DisplayedInvite.Uses.ToString();
                if (DisplayedInvite.MaxUses != 0) useField += "/" + DisplayedInvite.MaxUses;
                if (DisplayedInvite.Uses == 1)
                {
                    useField += " " + App.GetString("/Controls/UseSingular") + ", ";
                }
                else
                {
                    useField += " " + App.GetString("/Controls/UsePlural") + ", ";
                }

                // Temporary invite details
                if (DisplayedInvite.Temporary)
                {
                    TempInvite.Visibility = Visibility.Visible;
                    ToolTipService.SetToolTip(TempInvite, App.GetString("/Controls/InviteTempDesc"));
                }
                else
                {
                    TempInvite.Visibility = Visibility.Collapsed;
                }

                // Update expiration time
                var creationTime = DateTime.Parse(DisplayedInvite.CreatedAt);
                if (DisplayedInvite.MaxAge != 0)
                {
                    var timeDiff = TimeSpan.FromSeconds(DisplayedInvite.MaxAge -
                                                        DateTime.Now.Subtract(creationTime).TotalSeconds);
                    useField += App.GetString("/Controls/expiresIn").Replace("<time>", timeDiff.ToString(@"hh\:mm\:ss"));

                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Start();
                    timer.Tick += (sender, o) =>
                    {
                        var timeDiffLive =
                            TimeSpan.FromSeconds(DisplayedInvite.MaxAge -
                                                 DateTime.Now.Subtract(creationTime).TotalSeconds);
                        if (timeDiffLive.TotalSeconds == 0)
                            Description.Text = App.GetString("/Controls/ExpiredLink");
                        else
                            Description.Text = useField.Remove(useField.Length - 8) +
                                               timeDiffLive.ToString(@"hh\:mm\:ss");
                        timer.Start();
                    };
                }
                else
                {
                    useField += App.GetString("/Controls/expiresNever");
                }

                // Description
                Description.Text = useField;
            }
        }

        public InviteControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Delete Invite
        /// </summary>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteInvite?.Invoke(this.DataContext,null);
        }

        /// <summary>
        /// Copy invite code to clipboard
        /// </summary>
        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText("http://discord.gg/" + InviteCode.Text);
            Clipboard.SetContent(dp);
        }

        /// <summary>
        /// Share invite code
        /// </summary>
        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (manager, args) =>
            {
                args.Request.Data.SetWebLink(new Uri("http://discord.gg/" + InviteCode.Text));
                if (DisplayedInvite.Channel.Name != null)
                    args.Request.Data.Properties.Title = App.GetString("/Controls/InviteTo") + " #" + DisplayedInvite.Channel.Name;
                else
                    args.Request.Data.Properties.Title = App.GetString("/Controls/InviteTo") + " " + DisplayedInvite.Guild.Name;
            };
            DataTransferManager.ShowShareUI();
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dipose()
        {
            //Nothing to dispose
        }
    }
}
