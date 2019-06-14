using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.SharedModels;
using Quarrel.SimpleClasses;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class MemberControl : UserControl
    {
        /// <summary>
        /// API Data of Member to display
        /// </summary>
        public GuildMember RawMember
        {
            get => (GuildMember)GetValue(RawMemberProperty);
            set => SetValue(RawMemberProperty, value);
        }
        public static readonly DependencyProperty RawMemberProperty = DependencyProperty.Register(
            nameof(RawMember),
            typeof(GuildMember),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        // Presence Status of User to display
        public Presence Status
        {
            get {
                object pr = GetValue(PresenceProperty);
                if (pr != null) return (Presence)pr;
                else return new Presence();
            }
            set => SetValue(PresenceProperty, value);
        }
        public static readonly DependencyProperty PresenceProperty = DependencyProperty.Register(
            nameof(Status),
            typeof(Presence),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// Top role of Member
        /// </summary>
        public HoistRole Role
        {
            get => (HoistRole)GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }
        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            nameof(Role),
            typeof(HoistRole),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// True if the user is typing
        /// </summary>
        public bool IsTyping
        {
            get => (bool)GetValue(IsTypingProperty);
            set => SetValue(IsTypingProperty, value);
        }
        public static readonly DependencyProperty IsTypingProperty = DependencyProperty.Register(
            nameof(IsTyping),
            typeof(bool),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// User's name
        /// </summary>
        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register(
            nameof(Username),
            typeof(string),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MemberControl instance = d as MemberControl;
            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == UsernameProperty)
            {
                // Null check
                if (Username == null) return;

                // Update
                username.Text = Username;
            }
            else if (prop == RawMemberProperty)
            {
                // Null check
                if (RawMember == null) return;

                // Update Avatar
                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(RawMember.User.Avatar, RawMember.User.Id, "?size=64"));
                AvatarBG.Fill = RawMember.User.Avatar != null ? Common.GetSolidColorBrush("#00000000") : Common.DiscriminatorColor(RawMember.User.Discriminator);

                // Update details
                OwnerIndicator.Visibility = RawMember.User.Id == (App.CurrentGuildIsDM ? (App.CurrentChannelId != null ? LocalState.DMs[App.CurrentChannelId].OwnerId : "") : (App.CurrentGuildId != null ? LocalState.CurrentGuild.Raw.OwnerId : "")) ? Visibility.Visible : Visibility.Collapsed;
                BotIndicator.Visibility = RawMember.User.Bot ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (prop == IsTypingProperty)
            {
                // Update typing indicator visiblity 
                if (IsTyping)
                    ShowTyping.Begin();
                else
                    HideTyping.Begin();
            }
            else if (prop == PresenceProperty)
            {
                // Set Fill to appropiate brush
                if (Status.Status != null && Status.Status != "invisible")
                    rectangle.Fill = (SolidColorBrush)App.Current.Resources[Status.Status];
                else if (Status.Status == "invisible")
                    rectangle.Fill = (SolidColorBrush)App.Current.Resources["offline"];

                // Update Game display
                if (Status.Game != null)
                {
                    // Basic game details
                    playing.Visibility = Visibility.Visible;
                    game.Visibility = Visibility.Visible;
                    game.Text = Status.Game.Name;

                    // Game rich presence
                    if (Status.Game.State != null || Status.Game.Details != null || Status.Game.Assets != null)
                    {
                        game.Opacity = 1;
                        rich.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        game.Opacity = 0.6;
                        rich.Visibility = Visibility.Collapsed;
                    }

                    // Game type
                    switch (Status.Game.Type)
                    {
                        case 0:
                            playing.Text = App.GetString("/Controls/Playing");
                            break;
                        case 1:
                            playing.Text = App.GetString("/Controls/Streaming");
                            break;
                        case 2:
                            playing.Text = App.GetString("/Controls/ListeningTo");
                            break;
                        case 3:
                            playing.Text = App.GetString("/Controls/Watching");
                            break;
                    }
                }
                else
                {
                    // There's no game, hide everything
                    playing.Visibility = Visibility.Collapsed;
                    rich.Visibility = Visibility.Collapsed;
                    game.Visibility = Visibility.Collapsed;
                }
            }
            else if (prop == RoleProperty)
            {
                // Null check
                if (RawMember == null) return;

                // Update username color
                if (RawMember?.Roles != null)
                {
                    // False if the roles of the user don't override the default text color
                    bool overriden = false;
                    foreach (string role in RawMember.Roles)
                    {
                        if (LocalState.Guilds[App.CurrentGuildId].roles[role].Color != 0)
                        {
                            username.Foreground = Common.IntToColor(LocalState.Guilds[App.CurrentGuildId].roles[role].Color);
                            overriden = true;
                            break;
                        }
                    }
                    if (overriden == false)
                    {
                        username.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
                    }
                }
                else
                {
                    username.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
                }
            }
        }

        public MemberControl()
        {
            this.InitializeComponent();

            RightTapped += OpenMenuFlyout;
            Holding += OpenMenuFlyout;
        }

        /// <summary>
        /// Open Member Flyout (holding)
        /// </summary>
        private void OpenMenuFlyout(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
                App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, RawMember.User.Id, App.CurrentGuildId, e.GetPosition(this));
        }

        /// <summary>
        /// Open Member Flyout (right-tapped)
        /// </summary>
        private void OpenMenuFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
                App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, RawMember.User.Id, App.CurrentGuildId, e.GetPosition(this));
        }

        /// <summary>
        /// On unloading
        /// </summary>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        private void Dispose()
        {
            RightTapped -= OpenMenuFlyout;
            Holding -= OpenMenuFlyout;
        }
    }
}
