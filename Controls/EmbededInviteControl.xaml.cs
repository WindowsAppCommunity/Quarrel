using Discord_UWP.SharedModels;
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

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
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
        private async void LoadInvite(bool ForceJoin)
        {
            GuildName.Foreground = (SolidColorBrush)Application.Current.Resources["InvertedBG"];
            InviteCode = InviteCode.Replace(">\n", "");
            InviteCode = InviteCode.Replace("http://discord.me/","");
            InviteCode = InviteCode.Replace("https://discord.me/", "");
            InviteCode = InviteCode.Replace("https://discord.gg/", "");
            InviteCode = InviteCode.Replace("http://discord.gg/", "");
            InviteCode = InviteCode.Replace("https://discordapp.com/invite/", "");
            InviteCode = InviteCode.Replace("http://discordapp.com/invite/", "");

            GuildName.Opacity = 0;
            Loading.Opacity = 1;

            try
            {
                DisplayedInvite = await RESTCalls.GetInvite(InviteCode);
                if (DisplayedInvite == null)
                {
                    InvalidInvite(InvalidReason.Default);
                    return;
                }
                Loading.Fade(0, 200).Start();
                GuildName.Visibility = Visibility.Visible;
                TimeSpan timeDiff = TimeSpan.FromSeconds(1);
                if (DisplayedInvite.CreatedAt != null && DisplayedInvite.MaxAge != 0)
                {
                    var creationTime = DateTime.Parse(DisplayedInvite.CreatedAt);
                    timeDiff = TimeSpan.FromSeconds(DisplayedInvite.MaxAge -
                                        DateTime.Now.Subtract(creationTime).TotalSeconds);
                }

                if (DisplayedInvite.Guild == null)
                {
                    InvalidInvite(InvalidReason.Default);
                    return;
                }

                if (DisplayedInvite.Guild?.Icon != null)
                {
                    GuildImage.Visibility = Visibility.Visible;
                    GuildImageBrush.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/icons/"+DisplayedInvite.Guild.Id+"/"+DisplayedInvite.Guild.Icon+".png"));
                }
                else
                {
                    GuildImage.Visibility = Visibility.Collapsed;
                }

                ChannelName.Text = "#"+DisplayedInvite.Channel.Name;
                ChannelName.Fade(0.6f, 200).Start();
                GuildName.Fade(1,100).Start();
                GuildImage.Fade(1,300).Start();
                MemberCounters.Visibility = Visibility.Visible;
                MemberCounters.Fade(1, 400).Start();
                onlineCounter.Text = DisplayedInvite.OnlineCount + " online";
                offlineCounter.Text = DisplayedInvite.MemberCount + " members";
                if (LocalState.Guilds.ContainsKey(DisplayedInvite.Guild.Id) || ForceJoin)
                {
                    GuildName.Text = App.GetString("/Controls/InviteJoined") + " " + DisplayedInvite.Guild.Name;
                    Status = InviteStatus.AlreadyJoined;
                    return;
                }
                if (DisplayedInvite.MaxUses != 0 && DisplayedInvite.MaxUses <= DisplayedInvite.Uses)
                {
                    InvalidInvite(InvalidReason.MaxUses);
                    return;
                }
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
                InvalidInvite(InvalidReason.Default);
            }
        }

        enum InvalidReason { Default, MaxUses, Expired }
        private void InvalidInvite(InvalidReason reason)
        {
            Loading.Fade(0, 200).Start();
            GuildName.Fade(1, 350).Start();
            ChannelName.Visibility = Visibility.Collapsed;
            if(reason == InvalidReason.Default) GuildName.Text = App.GetString("/Controls/InviteInvalid");
            else if (reason == InvalidReason.MaxUses) GuildName.Text = App.GetString("/Controls/InviteMaxUses");
            else if(reason== InvalidReason.Expired) GuildName.Text = App.GetString("/Controls/InviteExpired");
            GuildName.Foreground = (SolidColorBrush) Application.Current.Resources["dnd"];
            MemberCounters.Visibility = Visibility.Collapsed;
            GuildImage.Visibility = Visibility.Collapsed;
            Status = InviteStatus.Invalid;

        }
        public EmbededInviteControl()
        {
            this.InitializeComponent();
            GatewayManager.Gateway.GuildDeleted += Gateway_GuildDeleted;
            GatewayManager.Gateway.GuildCreated += Gateway_GuildCreated;
        }

        private async void Gateway_GuildCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {
            if (DisplayedInvite?.Guild != null && e.EventData.Id == DisplayedInvite.Guild.Id)
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    LoadInvite(false);
                });
        }

        private async void Gateway_GuildDeleted(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.GuildDelete> e)
        {
            if (DisplayedInvite?.Guild != null && e.EventData.GuildId == DisplayedInvite.Guild.Id)
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    LoadInvite(false);
                });
        }

        public enum InviteStatus
        {
            Canjoin,
            AlreadyJoined,
            Invalid
        };

        public InviteStatus Status = InviteStatus.Invalid;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Status == InviteStatus.AlreadyJoined)
            {
                App.SelectGuildChannel(DisplayedInvite.Guild.Id, DisplayedInvite.Channel.Id);
            }
            else if (Status == InviteStatus.Canjoin)
            {
                Loading.Fade(1,200).Start();
                GuildImage.Fade(0.4f).Start();
                await RESTCalls.AcceptInvite(InviteCode);
                DisplayedInvite = await RESTCalls.GetInvite(InviteCode);
                LoadInvite(true);
            }
        }

        public void Dispose()
        {
            GatewayManager.Gateway.GuildDeleted -= Gateway_GuildDeleted;
            GatewayManager.Gateway.GuildCreated -= Gateway_GuildCreated;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
    }
}
