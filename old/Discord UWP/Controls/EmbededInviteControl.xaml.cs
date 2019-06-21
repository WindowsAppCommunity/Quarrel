using Microsoft.Toolkit.Uwp.UI.Animations;
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
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.SharedModels;
using DiscordAPI.API.Gateway;
using DiscordAPI.API.Gateway.DownstreamEvents;
using Guild = DiscordAPI.SharedModels.Guild;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class EmbededInviteControl : UserControl
    {
        public string InviteCode
        {
            get { return (string)GetValue(InviteCodeProperty); }
            set { SetValue(InviteCodeProperty, value); }
           
        }
        public static readonly DependencyProperty InviteCodeProperty = DependencyProperty.Register(
            nameof(InviteCode),
            typeof(string),
            typeof(EmbededInviteControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        public Invite DisplayedInvite;
        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as EmbededInviteControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == InviteCodeProperty)
            {
                LoadInvite(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Join">False if just rendering the embed</param>
        private async void LoadInvite(bool Join)
        {
            GuildName.Foreground = (SolidColorBrush)Application.Current.Resources["InvertedBG"];

            // Trim link prefix
            InviteCode = InviteCode.Replace(">\n", "");
            InviteCode = InviteCode.Replace("http://discord.me/","");
            InviteCode = InviteCode.Replace("https://discord.me/", "");
            InviteCode = InviteCode.Replace("https://discord.gg/", "");
            InviteCode = InviteCode.Replace("http://discord.gg/", "");
            InviteCode = InviteCode.Replace("https://discordapp.com/invite/", "");
            InviteCode = InviteCode.Replace("http://discordapp.com/invite/", "");

            // Show loading
            GuildName.Opacity = 0;
            Loading.Opacity = 1;

            try
            {
                // Get invite
                DisplayedInvite = await RESTCalls.GetInvite(InviteCode);
                if (DisplayedInvite == null)
                {
                    InvalidInvite(InvalidReason.Default);
                    return;
                }
                
                // Show invite
                Loading.Fade(0, 200).Start();
                GuildName.Visibility = Visibility.Visible;

                // Get expiration time
                TimeSpan timeDiff = TimeSpan.FromSeconds(1);
                if (DisplayedInvite.CreatedAt != null && DisplayedInvite.MaxAge != 0)
                {
                    var creationTime = DateTime.Parse(DisplayedInvite.CreatedAt);
                    timeDiff = TimeSpan.FromSeconds(DisplayedInvite.MaxAge -
                                        DateTime.Now.Subtract(creationTime).TotalSeconds);
                }

                // Return an error if the guild is null
                if (DisplayedInvite.Guild == null)
                {
                    InvalidInvite(InvalidReason.Default);
                    return;
                }

                // Display the guild image
                if (DisplayedInvite.Guild?.Icon != null)
                {
                    GuildImage.Visibility = Visibility.Visible;
                    GuildImageBrush.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/icons/"+DisplayedInvite.Guild.Id+"/"+DisplayedInvite.Guild.Icon+".png"));
                }
                else
                {
                    GuildImage.Visibility = Visibility.Collapsed;
                }

                // Setup channel details (invites are technically to a channel)
                ChannelName.Text = "#"+DisplayedInvite.Channel.Name;
                ChannelName.Fade(0.6f, 200).Start();
                
                // Guild details visibility 
                GuildName.Fade(1,100).Start();
                GuildImage.Fade(1,300).Start();

                // Member values
                MemberCounters.Visibility = Visibility.Visible;
                MemberCounters.Fade(1, 400).Start();
                onlineCounter.Text = DisplayedInvite.OnlineCount + " online";
                offlineCounter.Text = DisplayedInvite.MemberCount + " members";

                // Return error if the user has already joined
                if (LocalState.Guilds.ContainsKey(DisplayedInvite.Guild.Id) || Join)
                {
                    GuildName.Text = App.GetString("/Controls/InviteJoined") + " " + DisplayedInvite.Guild.Name;
                    Status = InviteStatus.AlreadyJoined;
                    return;
                }

                // Return error if the invite uses has already been maxed
                if (DisplayedInvite.MaxUses != 0 && DisplayedInvite.MaxUses <= DisplayedInvite.Uses)
                {
                    InvalidInvite(InvalidReason.MaxUses);
                    return;
                }
                
                // Return error if the invite has timed out
                if (timeDiff.TotalSeconds > 0)
                {
                    GuildName.Text = App.GetString("/Controls/InviteJoin") + " " + DisplayedInvite.Guild.Name;
                    Status = InviteStatus.Canjoin;
                }
                else
                {
                    InvalidInvite(InvalidReason.Default);
                }
            }
            catch
            {
                // Something went wrong, return an error
                InvalidInvite(InvalidReason.Default);
            }
        }

        /// <summary>
        /// Handled reasons for failure
        /// </summary>
        enum InvalidReason { Default, MaxUses, Expired }
        private void InvalidInvite(InvalidReason reason)
        {
            // Hide loading indicator
            Loading.Fade(0, 200).Start();
            GuildName.Fade(1, 350).Start();
           
            // Show appropiate error text
            if(reason == InvalidReason.Default) GuildName.Text = App.GetString("/Controls/InviteInvalid");
            else if (reason == InvalidReason.MaxUses) GuildName.Text = App.GetString("/Controls/InviteMaxUses");
            else if(reason== InvalidReason.Expired) GuildName.Text = App.GetString("/Controls/InviteExpired");

            // Make the text red
            GuildName.Foreground = (SolidColorBrush) Application.Current.Resources["dnd"];

            // If there's a user linked to the invite, display a link to them so you can ask them for a link
            if (DisplayedInvite?.Inviter != null && LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(DisplayedInvite?.Inviter.Id))
            {
                ChannelName.Text = "Ask @" + DisplayedInvite.Inviter.Username + "#" + DisplayedInvite.Inviter.Discriminator + " for a new one";
                ChannelName.Visibility = Visibility.Visible;
            }
            else
            {
                ChannelName.Text = "";
                ChannelName.Visibility = Visibility.Collapsed;
            }

            // Hide Guild details (there are none)
            MemberCounters.Visibility = Visibility.Collapsed;
            GuildImage.Visibility = Visibility.Collapsed;

            // Interaction status is invalid
            Status = InviteStatus.Invalid;
        }

        public EmbededInviteControl()
        {
            this.InitializeComponent();
            GatewayManager.Gateway.GuildDeleted += Gateway_GuildDeleted;
            GatewayManager.Gateway.GuildCreated += Gateway_GuildCreated;
        }

        private async void Gateway_GuildCreated(object sender, GatewayEventArgs<Guild> e)
        {
            // If the guild is this guild
            if (DisplayedInvite?.Guild != null && e.EventData.Id == DisplayedInvite.Guild.Id)
                // Run on UI thread
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Update the info to say it's already joined
                    LoadInvite(false);
                });
        }

        private async void Gateway_GuildDeleted(object sender, GatewayEventArgs<GuildDelete> e)
        {
            // If the guild is this guild
            if (DisplayedInvite?.Guild != null && e.EventData.GuildId == DisplayedInvite.Guild.Id)
                // Run on UI thread
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Display the invite
                    LoadInvite(false);
                });
        }

        /// <summary>
        /// Interaction statuses
        /// </summary>
        public enum InviteStatus
        {
            Canjoin,
            AlreadyJoined,
            Invalid
        };

        public InviteStatus Status = InviteStatus.Invalid;

        /// <summary>
        /// When the invite is clicked
        /// </summary>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Move to already joined server
            if (Status == InviteStatus.AlreadyJoined)
            {
                App.SelectGuildChannel(DisplayedInvite.Guild.Id, DisplayedInvite.Channel.Id);
            }
            // Accept invite and rerender
            else if (Status == InviteStatus.Canjoin)
            {
                Loading.Fade(1,200).Start();
                GuildImage.Fade(0.4f).Start();
                await RESTCalls.AcceptInvite(InviteCode);
                DisplayedInvite = await RESTCalls.GetInvite(InviteCode);
                LoadInvite(true);
            }
            // Draft a message mentioning the Inviter
            else if(Status == InviteStatus.Invalid && ChannelName.Visibility == Visibility.Visible)
            {
                App.SelectGuildChannel(App.CurrentGuildId,App.CurrentChannelId, "@"+DisplayedInvite.Inviter.Username+"#"+DisplayedInvite.Inviter.Discriminator);
            }
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            GatewayManager.Gateway.GuildDeleted -= Gateway_GuildDeleted;
            GatewayManager.Gateway.GuildCreated -= Gateway_GuildCreated;
        }

        /// <summary>
        /// Unloaded
        /// </summary>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
    }
}
