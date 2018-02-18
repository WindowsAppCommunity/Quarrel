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
            InviteCode = InviteCode.Replace(">\n", "");
            InviteCode = InviteCode.Replace("http://discord.me/","");
            InviteCode = InviteCode.Replace("https://discord.me/", "");
            InviteCode = InviteCode.Replace("https://discord.gg/", "");
            InviteCode = InviteCode.Replace("http://discord.gg/", "");
            InviteCode = InviteCode.Replace("https://discordapp.com/invite/", "");
            InviteCode = InviteCode.Replace("http://discordapp.com/invite/", "");

            GreenIcon.Opacity = 0;
            RedIcon.Opacity = 0;
            JoinIcon.Opacity = 0;
            ChannelName.Opacity = 0;
            GuildName.Opacity = 0;
            Loading.Opacity = 1;

            try
            {
                DisplayedInvite = await RESTCalls.GetInvite(InviteCode);
                Loading.Fade(0, 200).Start();
                GuildName.Visibility = Visibility.Visible;
                TimeSpan timeDiff = TimeSpan.FromSeconds(1);
                if (DisplayedInvite.CreatedAt != null && DisplayedInvite.MaxAge != 0)
                {
                    var creationTime = DateTime.Parse(DisplayedInvite.CreatedAt);
                    timeDiff = TimeSpan.FromSeconds(DisplayedInvite.MaxAge -
                                        DateTime.Now.Subtract(creationTime).TotalSeconds);
                }

                GuildName.Text = DisplayedInvite.Guild.Name;
                GuildName.Fade(1, 350).Start();
                ChannelName.Fade(1, 200).Start();
                if (LocalState.Guilds.ContainsKey(DisplayedInvite.Guild.Id) || ForceJoin)
                {
                    GreenIcon.Fade(1, 200).Start();
                    ChannelName.Text = App.GetString("/Controls/InviteJoined") + " " + DisplayedInvite.Channel.Name;
                    return;
                }
                if (DisplayedInvite.MaxUses != 0 && DisplayedInvite.MaxUses <= DisplayedInvite.Uses)
                {
                    ChannelName.Text = App.GetString("/Controls/InviteMaxUses");
                    RedIcon.Fade(1, 200).Start();
                    return;
                }
                if (timeDiff.TotalSeconds > 0)
                {
                    JoinIcon.Fade(1, 200).Start();
                    ChannelName.Text = App.GetString("/Controls/InviteJoin") + " " + DisplayedInvite.Channel.Name;
                }
                else
                {
                    RedIcon.Fade(1, 200).Start();
                    ChannelName.Text = App.GetString("/Controls/InviteExpired");
                }
            }
            catch
            {
                Loading.Fade(0, 200).Start();
                GuildName.Fade(1, 350).Start();
                ChannelName.Fade(1, 200).Start();
                RedIcon.Fade(1, 200).Start();
                ChannelName.Text = App.GetString("/Controls/InviteInvalid");
                GuildName.Visibility = Visibility.Collapsed;
            }
        }

        public EmbededInviteControl()
        {
            this.InitializeComponent();
            GatewayManager.Gateway.GuildDeleted += Gateway_GuildDeleted;
            GatewayManager.Gateway.GuildCreated += Gateway_GuildCreated;
        }

        private async void Gateway_GuildCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.EventData.Id == DisplayedInvite.Guild.Id)
                    LoadInvite(false);
            });
        }

        private async void Gateway_GuildDeleted(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.GuildDelete> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.EventData.GuildId == DisplayedInvite.Guild.Id)
                    LoadInvite(false);
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (RedIcon.Opacity == 1) return;
            if (GreenIcon.Opacity == 1)
                App.NavigateToGuild(DisplayedInvite.Guild.Id);
            if(JoinIcon.Opacity == 1)
            {
                GreenIcon.Opacity = 0;
                RedIcon.Opacity = 0;
                JoinIcon.Opacity = 0;
                ChannelName.Opacity = 0;
                GuildName.Opacity = 0;
                Loading.Opacity = 1;

                //DisplayedInvite = await RESTCalls.AcceptInvite(InviteCode); I guess this doesn't work or something...
                await RESTCalls.AcceptInvite(InviteCode);
                DisplayedInvite = await RESTCalls.GetInvite(InviteCode);
                DisplayedInvite.Uses++;
                LoadInvite(true);
            }
        }
    }
}
