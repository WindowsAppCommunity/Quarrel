// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using System;
using System.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace Discord_UWP.Controls
{
    public sealed partial class VoiceMemberControl : UserControl
    {
        private GuildMember member;

        public VoiceState DisplayedUser
        {
            get { return (VoiceState)GetValue(DisplayedUserProperty); }
            set { SetValue(DisplayedUserProperty, value); }
        }
        public static readonly DependencyProperty DisplayedUserProperty = DependencyProperty.Register(
            nameof(DisplayedUser),
            typeof(VoiceState),
            typeof(VoiceMemberControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VoiceMemberControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedUserProperty)
            {
                member = LocalState.Guilds[App.CurrentGuildId].members[DisplayedUser.UserId];

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

                if (DisplayedUser.SelfDeaf || DisplayedUser.ServerDeaf)
                {
                    if (DisplayedUser.ServerDeaf)
                    {
                        ServerDeaf.Begin();
                    }
                    else
                    {
                        LocalDeaf.Begin();
                    }
                    ShowDeaf.Begin();
                }
                else
                {
                    HideDeaf.Begin();
                }

                if (DisplayedUser.SelfMute || DisplayedUser.ServerMute)
                {
                    if (DisplayedUser.ServerMute)
                    {
                        ServerMute.Begin();
                    }
                    else
                    {
                        LocalMute.Begin();
                    }
                    ShowMute.Begin();
                }
                else
                {
                    HideMute.Begin();
                }

                //discriminator.Text = "#" + DisplayedFriend.User.Discriminator;
            }
        }

        public VoiceMemberControl()
        {
            this.InitializeComponent();
            Tapped += OpenMemberFlyout;
            RightTapped += OpenVoiceFlyout;
            Holding += OpenVoiceFlyout;
            Managers.GatewayManager.Gateway.VoiceStateUpdated += Gateway_VoiceStateUpdated;
            Managers.VoiceManager.ConnectoToVoiceHandler += VoiceManager_ConnectoToVoiceHandler;
        }

        private void OpenVoiceFlyout(object sender, HoldingRoutedEventArgs e)
        {
            //TODO: Voice managment flyout
        }

        private void OpenVoiceFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            //TODO: Voice managment flyout
        }

        private async void VoiceManager_ConnectoToVoiceHandler(object sender, Managers.VoiceManager.ConnectToVoiceArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (e.ChannelId == DisplayedUser.ChannelId)
                     {
                         Managers.VoiceManager.VoiceConnection.Speak += VoiceConnection_Speak;
                     }
                     else
                     {
                         Managers.VoiceManager.VoiceConnection.Speak -= VoiceConnection_Speak;
                     }
                 });
        }

        private async void VoiceConnection_Speak(object sender, Voice.VoiceConnectionEventArgs<Voice.DownstreamEvents.Speak> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (e.EventData.UserId == DisplayedUser.UserId)
                     {
                         if (e.EventData.Speaking)
                         {
                             Speaking.Begin();
                         }
                         else
                         {
                             StopSpeaking.Begin();
                         }
                     }
                 });
        }

        private async void Gateway_VoiceStateUpdated(object sender, Gateway.GatewayEventArgs<VoiceState> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (e.EventData.UserId == DisplayedUser.UserId)
                     {
                         DisplayedUser = e.EventData;
                         OnPropertyChanged(null, DisplayedUserProperty);
                     }
                 });
        }

        private void OpenMemberFlyout(object sender, TappedRoutedEventArgs e)
        {
            App.ShowMemberFlyout(this, member.User);
        }
    }

    public class VoiceMemberContainer : INotifyPropertyChanged
    {
        private VoiceState voiceState;
        public VoiceState VoiceState
        {
            get { return voiceState; }
            set { if (voiceState != null && voiceState.Equals(value)) return; voiceState = value; OnPropertyChanged("VoiceState"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
