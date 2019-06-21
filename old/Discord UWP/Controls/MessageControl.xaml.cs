using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using static Quarrel.Common;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Media.Animation;
using Quarrel.Controls;
using static Quarrel.Managers.MessageManager;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.System;
using DiscordAPI.API.Gateway;
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.SharedModels;
using Quarrel.SimpleClasses;
using DiscordAPI.API.Gateway.DownstreamEvents;

namespace Quarrel.Controls
{
    public sealed partial class MessageControl : UserControl
    {

        /// <summary>
        /// Is the more button visible?
        /// </summary>
        public Visibility MoreButtonVisibility
        {
            get { return moreButton.Visibility; }
            set { moreButton.Visibility = value; }
        }

        /// <summary>
        /// Is the message an advert?
        /// </summary>
        public MessageTypes MessageType
        {
            get { return (MessageTypes)GetValue(MessageTypeProperty); }
            set { SetValue(MessageTypeProperty, value); }
        }
        public static readonly DependencyProperty MessageTypeProperty = DependencyProperty.Register(
            nameof(MessageType),
            typeof(MessageTypes),
            typeof(MessageControl),
            new PropertyMetadata(MessageTypes.Default, OnPropertyChangedStatic));

        /// <summary>
        /// Is the message the continuation of another one?
        /// </summary>
        public bool IsContinuation
        {
            get { return (bool)GetValue(IsContinuationProperty); }
            set { SetValue(IsContinuationProperty, value); }
        }
        public static readonly DependencyProperty IsContinuationProperty = DependencyProperty.Register(
            nameof(IsContinuation),
            typeof(bool),
            typeof(MessageControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// Ture if the message is pending being sent (depricated)
        /// </summary>
        public bool IsPending
        {
            get { return (bool)GetValue(IsPendingProperty); }
            set { SetValue(IsPendingProperty, value); }
        }
        public static readonly DependencyProperty IsPendingProperty = DependencyProperty.Register(
            nameof(IsPending),
            typeof(bool),
            typeof(MessageControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// True if the message has been edited
        /// </summary>
        public bool Edited
        {
            get { return (bool)GetValue(EditedProperty); }
            set { SetValue(EditedProperty, value); }
        }
        public static readonly DependencyProperty EditedProperty = DependencyProperty.Register(
            nameof(Edited),
            typeof(bool),
            typeof(MessageControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// True if the current user has blocked the user that posted the message
        /// </summary>
        public bool IsBlocked
        {
            get { return (bool)GetValue(IsBlockedProperty); }
            set { SetValue(IsBlockedProperty, value); }
        }
        public static readonly DependencyProperty IsBlockedProperty = DependencyProperty.Register(
            nameof(IsBlocked),
            typeof(bool),
            typeof(MessageControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// The header of the messages, that can indicate data such as "new messages" or the date (depricated)
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(MessageControl),
            new PropertyMetadata(string.Empty, OnPropertyChangedStatic));

        /// <summary>
        /// True if it was the last read message in the channel before being opened
        /// </summary>
        public bool LastRead
        {
            get { return (bool)GetValue(LastReadProperty); }
            set { SetValue(LastReadProperty, value); }
        }
        public static readonly DependencyProperty LastReadProperty = DependencyProperty.Register(
            nameof(LastRead),
            typeof(bool),
            typeof(MessageControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// The message to be displayed
        /// </summary>
        public Message Message
        {
            get { return (Message)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message),
            typeof(Message),
            typeof(MessageControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));


        /// <summary>
        /// Calls OnPropertyChanged for this instance of the control
        /// </summary>
        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as MessageControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        AdControl advert;
        string originalcontent = "";

        /* IT IS VERY IMPORTANT TO REMEMBER THE MESSAGECONTROL GET RECYCLED BY VIRTUALIZATION, AND THAT VALUES MUST SYSTEMATICALLY BE RESET
         * For example, if a color property is changed depending on a boolean property, make sure to include an `else` that will reset the color */
        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            // If the status of the message being a continuation is updated
            if (prop == IsContinuationProperty)
            {
                // Make continuation
                if (IsContinuation)
                    VisualStateManager.GoToState(((MessageControl)d), "Continuation", false);
                // Go to default
                else
                    VisualStateManager.GoToState(((MessageControl)d), "VisualState", false);
            }

            // Enter compact
            if (Storage.Settings.CompactMode)
                VisualStateManager.GoToState(((MessageControl)d), "Compact", false);
            // Leave compact
            else
                VisualStateManager.GoToState(((MessageControl)d), "VisualState", false);

            // If message type changed
            if (prop == MessageTypeProperty)
            {
                // Is advert (depricated)
                if (MessageType == MessageTypes.Advert)
                {
                    if (rootGrid.Children.Contains(reactionView))
                        rootGrid.Children.Remove(reactionView);
                    VisualStateManager.GoToState(this, "Advert", false);
                    advert = new AdControl();
                    advert.HorizontalAlignment = HorizontalAlignment.Center;
                    advert.Width = 300;
                    advert.Height = 50;
                    advert.ApplicationId = "9nbrwj777c8r";
                    advert.AdUnitId = "336795";
                    advert.Margin = new Thickness(6);
                    advert.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    Grid.SetColumnSpan(advert, 10);
                    Grid.SetRowSpan(advert, 10);
                    rootGrid.Children.Add(advert);
                    if (reactionView != null)
                        rootGrid.Children.Remove(reactionView);
                    return;
                }
                else
                {
                    // Remove advert control
                    if (rootGrid.Children.Contains(advert))
                        rootGrid.Children.Remove(advert);
                    advert = null;

                    // Recipient Added to Group DM message
                    if (MessageType == MessageTypes.RecipientAdded)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["online"]);
                        content.Text = "**" + Message.User.Username + "** " + App.GetString("/Controls/AddedUser") + App.GetString("/Controls/ToTheConversation").Replace("<user>", "**" + Message.Mentions.First().Username + "**");
                    }
                    // Recipient Removed to Group DM message
                    else if (MessageType == MessageTypes.RecipientRemoved)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["dnd"]);
                        content.Text = "**" + Message.User.Username + "** " + App.GetString("/Controls/RemovedUser") + App.GetString("/Controls/FromTheConversation").Replace("<user>", "**" + Message.Mentions.First().Username + "**");
                    }
                    // Channel Icon Changed message
                    else if (MessageType == MessageTypes.ChannelIconChanged)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["InvertedBG"]);
                        content.Text = "**" + Message.User.Username + "** changed the channel's icon";
                    }
                    // Channel Name Changed message
                    else if (MessageType == MessageTypes.ChannelNameChanged)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["InvertedBG"]);
                        content.Text = "**" + Message.User.Username + "** changed the channel's name";
                    }
                    // Call message
                    else if (MessageType == MessageTypes.Call)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        if (Message.Call == null || !Message.Call.Participants.Contains(LocalState.CurrentUser.Id))
                        {
                            //Missed call
                            SetAltIcon("", (SolidColorBrush)App.Current.Resources["InvertedBG"], 18, true);
                            AlternativeIcon.Glyph = "";
                            AlternativeIcon.Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"];
                            content.Text = App.GetString("/Controls/YouMissedACall").Replace("<user>", "**" + Message.User.Username + "**");
                        }
                        else
                        {
                            //Answered call
                            SetAltIcon("", (SolidColorBrush)App.Current.Resources["online"]);
                            if (Message.User.Id == LocalState.CurrentUser.Id)
                                content.Text = App.GetString("/Controls/You") + " " + App.GetString("/Controls/StartedACall");
                            else
                                content.Text = App.GetString("/Controls/CallStartedBy").Replace("<user>", "**" + Message.User.Username + "**");
                        }
                        AlternativeIcon.FontSize = 18;

                        //content.Text = App.GetString("/Controls/YouMissedACall") + " **" + Message.Value.User.Username + "**";
                    }
                    // Message Pinned message
                    else if (MessageType == MessageTypes.PinnedMessage)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        if (rootGrid.Children.Contains(advert))
                            rootGrid.Children.Remove(advert);
                        advert = null;
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["InvertedBG"]);
                        content.Text = "**" + Message.User.Username + "** " + App.GetString("/Controls/PinnedAMessageInThisChannel");
                    }
                    // Member Joined message
                    else if (MessageType == MessageTypes.GuildMemberJoined)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["online"]);
                        content.Text = "**" + Message.User.Username + "**" + " joined the server!";
                    }
                    // Revert to standard Message template
                    else if (MessageType == MessageTypes.Default)
                    {
                        if (IsContinuation)
                            VisualStateManager.GoToState(((MessageControl)d), "Continuation", false);
                        else
                            VisualStateManager.GoToState(((MessageControl)d), "VisualState", false);
                    }
                }
            }
            // Message changed
            if (prop == MessageProperty)
            {
                UpdateMessage(Edited);
            }
            // Last Read updated
            if (prop == LastReadProperty)
            {
                if (LastRead)
                {
                    // Show "NEW MESSAGES" header
                    HeaderUI.Visibility = Visibility.Visible;
                }
                else
                {
                    // Hide "NEW MESSAGES" header
                    HeaderUI.Visibility = Visibility.Collapsed;
                }
            }
            // Is Pending updated
            if (prop == IsPendingProperty)
            {
                if (IsPending)
                {
                    content.Opacity = 0.5;
                }
                else
                {
                    content.Opacity = 1;
                }
            }
            // Is Blocked updated
            if (prop == IsBlockedProperty)
            {
                if (IsBlocked)
                {
                    // Hide content, show blocked message
                    content.Visibility = Visibility.Collapsed;
                    BlockedMessage.Visibility = Visibility.Visible;
                }
                else
                {
                    // Show content, hide blocked message
                    content.Visibility = Visibility.Visible;
                    BlockedMessage.Visibility = Visibility.Collapsed;
                }
            }
        }

        public MessageControl()
        {
            this.InitializeComponent();

            // Somehow the Gatewat can be null when some messages are loaded. Just ignore it in that case
            if (GatewayManager.Gateway != null)
            {
                GatewayManager.Gateway.MessageReactionAdded += GatewayOnMessageReactionAdded;
                GatewayManager.Gateway.MessageReactionRemoved += GatewayOnMessageReactionRemoved;
            }
        }

        /// <summary>
        /// Set Icon for non-default message type
        /// </summary>
        /// <param name="glyph">Icon</param>
        /// <param name="color">Color</param>
        /// <param name="fontsize">FontSize</param>
        /// <param name="mirrored">True if the glyoh should be mirrored from it's usual direction</param>
        private void SetAltIcon(string glyph, SolidColorBrush color, int fontsize = 18, bool mirrored = false)
        {
            // Set icon
            AlternativeIcon.Glyph = glyph;
            AlternativeIcon.FontSize = fontsize;
            AlternativeIcon.Foreground = color;

            // Mirror glyph
            AlternativeIcon.RenderTransform = mirrored ? new ScaleTransform() { ScaleX = -1, CenterX = 9 } : new ScaleTransform() { ScaleX = 1 };
        }

        /// <summary>
        /// Triggered when Gateway Reaction Removed event is recieved
        /// </summary>
        private async void GatewayOnMessageReactionRemoved(object sender, GatewayEventArgs<MessageReactionUpdate> gatewayEventArgs)
        {
            // If not this message, forget it
            if (gatewayEventArgs.EventData.MessageId != messageid) return;

            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    // Shouldn't be true but can get weird with Visualization recycling
                    if (reactionView == null)
                        reactionView = GenerateWrapGrid();

                    // The button to remove or decriment from
                    ToggleButton toRemove = null;

                    // Check all reactions for removed reaction
                    foreach (ToggleButton toggle in reactionView.Children)
                    {
                        // Get Button Data
                        var tuple = toggle.Tag as Tuple<string, string, Reactions>;

                        // If the button is for the emoji
                        if (tuple.Item3.Emoji.Name == gatewayEventArgs.EventData.Emoji.Name)
                        {
                            var tb = ((toggle.Content as StackPanel).Children.Last() as TextBlock);
                            var text = tb.Text;
                            //var rt = ((toggle.Content as StackPanel).Children.Last() as TextBlock).RenderTransform = new TranslateTransform();
                            if (text == "1")
                            {
                                // If the reaction had only 1 instance, remove it
                                toRemove = toggle;
                                break;
                            }

                            // Uncheck button if user had checked it
                            if (tuple.Item3.Me)
                                toggle.IsChecked = false;

                            // Animate Text Change
                            AnimationSet.UseComposition = true;
                            await tb.Offset(22, -18, 150, 0, EasingType.Back, EasingMode.EaseIn).StartAsync();
                            tb.Text = (Convert.ToInt32(text) - 1).ToString();
                            await tb.Offset(22, 18, 0).StartAsync();
                            await tb.Offset(22, 0, 180, 0, EasingType.Back, EasingMode.EaseOut).StartAsync();
                            AnimationSet.UseComposition = false;
                        }
                    }

                    // Remove button
                    if (toRemove != null)
                        reactionView.Children.Remove(toRemove);
                });
        }

        /// <summary>
        /// Triggered when Gateway Reaction Added event is recieved
        /// </summary>
        private async void GatewayOnMessageReactionAdded(object sender, GatewayEventArgs<MessageReactionUpdate> gatewayEventArgs)
        {
            // If not this message, forget it
            if (gatewayEventArgs.EventData.MessageId != messageid) return;

            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    if (reactionView == null)
                    {
                        reactionView = GenerateWrapGrid();
                        rootGrid.Children.Add(reactionView);
                    }

                    // True if the button was available to be incrimented
                    bool success = false;

                    foreach (ToggleButton toggle in reactionView.Children)
                    {
                        // Get Button Data
                        var tuple = toggle.Tag as Tuple<string, string, Reactions>;

                        // If the Button is for the Emoji
                        if (tuple.Item3.Emoji.Name == gatewayEventArgs.EventData.Emoji.Name)
                        {
                            // Button found
                            success = true;

                            // Update text
                            var tb = ((toggle.Content as StackPanel).Children.Last() as TextBlock);
                            var text = tb.Text;
                            if (tuple.Item3.Me)
                                toggle.IsChecked = true;

                            // Animate Text Change
                            AnimationSet.UseComposition = true;
                            await tb.Offset(22, 18, 180, 0, EasingType.Back, EasingMode.EaseIn).StartAsync();
                            tb.Text = (Convert.ToInt32(text) + 1).ToString();
                            await tb.Offset(22, -18, 0).StartAsync();
                            await tb.Offset(22, 0, 180, 0, EasingType.Back, EasingMode.EaseOut).StartAsync();
                            AnimationSet.UseComposition = false;
                        }
                    }

                    // If there wasn't a button to be incrimented
                    if (!success)
                    {
                        var data = gatewayEventArgs.EventData;
                        Grid.SetRow(reactionView, 3);
                        Grid.SetColumn(reactionView, 1);

                        var toggle = GenerateReactionToggle(new Reactions()
                        {
                            Count = 1,
                            Emoji = data.Emoji,
                            Me = LocalState.CurrentUser.Id.Equals(data.UserId),
                        });
                        reactionView.Children.Add(toggle);
                    }
                });
        }

        /// <summary>
        /// WrapPanel containing reactions
        /// </summary>
        private WrapPanel reactionView;

        // ID of Author
        private string userid = "";
        
        // ID of Message
        public string messageid = "";

        /// <summary>
        /// Generate Slightly modified WrapPanel
        /// </summary>
        /// <returns>Generated WrapPanel</returns>
        public WrapPanel GenerateWrapGrid()
        {
            var wg = new WrapPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(6, 4, 0, 0),
                HorizontalSpacing = 4,
                VerticalSpacing = 4
            };
            return wg;
        }

        /// <summary>
        /// Update message properties
        /// </summary>
        /// <param name="edited">True if the message edited</param>
        public void UpdateMessage(bool edited = false)
        {
            // Reset fontsize and padding
            content.FontSize = Storage.Settings.MSGFontSize;
            content.Padding = new Thickness(6, 3, 3, content.FontSize / 4 + 1);

            // Clear ad
            if (rootGrid.Children.Contains(advert))
                rootGrid.Children.Remove(advert);
            advert = null;

            // If it's an enitrely new message, remove reactions
            if (!edited && rootGrid.Children.Contains(reactionView))
                rootGrid.Children.Remove(reactionView);

            // New message is not null
            if (Message != null)
            {
                // Set Message Id
                messageid = Message.Id;

                // Adjust Background for mention indication
                if (MessageType == MessageTypes.Default && (Message.MentionEveryone || Message.Mentions?.FirstOrDefault(x => x.Id == LocalState.CurrentUser.Id) != null))
                {
                    content.Background = GetSolidColorBrush("#14FAA61A");
                    content.BorderBrush = GetSolidColorBrush("#FFFAA61A");
                    content.BorderThickness = new Thickness(2, 0, 0, 0);
                }
                else
                {
                    content.Background = null;
                    content.BorderBrush = null;
                    content.BorderThickness = new Thickness(0);
                }

                // If the message has no author return
                if (Message.User == null) return;

                // Set Author by username
                if (Message.User.Username != null)
                    username.Content = Message.User.Username;
                else
                    username.Content = "";

                // Override Author by Nickname
                // Author as GuildMember
                GuildMember member;
                if (Message.User.Id != null) userid = Message.User.Id;
                else userid = "";
                if (App.CurrentGuildId != null && Message.User.Id != null)
                {
                    // Get Member by userId
                    if (LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(Message.User.Id))
                    {
                        member = LocalState.Guilds[App.CurrentGuildId].members[Message.User.Id];
                    }
                    else
                    {
                        member = new GuildMember() { User = Message.User };
                    }
                }
                else
                {
                    member = new GuildMember();
                }
                if (member.Nick != null)
                {
                    username.Content = member.Nick;
                }

                // Handle Message embeded activies
                if (Message.Activity != null)
                {
                    if (Message.Activity.Type == 3)
                    {
                        //Spotify
                    }
                }

                // Adjust Author color for Roles
                if (member.Roles != null && member.Roles.Any())
                {
                    bool changed = false;
                    foreach (var role in member.Roles)
                        if (LocalState.Guilds[App.CurrentGuildId].roles[role].Color != 0)
                        {
                            username.Foreground = IntToColor(LocalState.Guilds[App.CurrentGuildId].roles[role].Color);
                            changed = true;
                            break;
                        }
                    if (changed == false)
                        username.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
                }
                else
                {
                    username.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
                }

                // Toggle bot indicator
                if (Message.User.Bot == true)
                    BotIndicator.Visibility = Visibility.Visible;
                else
                    BotIndicator.Visibility = Visibility.Collapsed;

                // Toggle pin/unpin message on pin/unpin button
                if (Message.Pinned)
                    MorePin.Text = App.GetString("/Controls/Unpin");
                else
                    MorePin.Text = App.GetString("/Controls/Pin") + " ";

                // Toggle dev tool visibilty
                if (Storage.Settings.DevMode)
                    MoreCopyId.Visibility = MoreDevSplit.Visibility = Visibility.Visible;
                else
                    MoreCopyId.Visibility = MoreDevSplit.Visibility = Visibility.Collapsed;

                // Set Author Avatar
                AvatarBrush.ImageSource = new BitmapImage(Common.AvatarUri(Message.User.Avatar, Message.User.Id));

                // Set Author Avatar backdrop
                if (Message.User.Avatar == null)
                    AvatarBG.Fill = Common.DiscriminatorColor(Message.User.Discriminator);
                else
                    AvatarBG.Fill = Common.GetSolidColorBrush("#00000000");

                // Set timestamps
                timestamp.Text = Common.HumanizeDate(Message.Timestamp, null);
                if (Message.EditedTimestamp.HasValue)
                    timestamp.Text += " (" + App.GetString("/Controls/Edited") + " " + Common.HumanizeEditedDate(Message.EditedTimestamp.Value,
                                          Message.Timestamp) + ")";

                // Load Attachments
                LoadEmbedsAndAttachements();

                if (!edited)
                {
                    // Load reactions
                    if (Message.Reactions != null)
                    {
                        reactionView = GenerateWrapGrid();
                        foreach (Reactions reaction in Message.Reactions.Where(x => x.Count > 0))
                        {
                            reactionView.Children.Add(GenerateReactionToggle(reaction));
                        }

                        Grid.SetRow(reactionView, 4);
                        Grid.SetColumn(reactionView, 1);
                        rootGrid.Children.Add(reactionView);
                    }
                    else
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        reactionView = null;
                    }
                }

                // Give Markdown parser Mentioned members
                content.Users = Message.Mentions;

                // Toggle content visiblity
                if (Message?.Content == "")
                {
                    content.Visibility = Visibility.Collapsed;
                    Grid.SetRow(moreButton, 4);
                }
                else
                {
                    content.Visibility = Visibility.Visible;
                    Grid.SetRow(moreButton, 2);
                }
                // Update content
                content.Text = Message.Content;

                // Handle invite ocdes
                Regex regex = new Regex("(discord\\.(gg|io|me|li)\\/|discordapp\\.com\\/invite\\/)(([A-Za-z]|[0-9])+)");
                foreach (Match match in regex.Matches(content.Text))
                {
                    if (match.Groups.Count >= 3)
                    {
                        EmbedViewer.Visibility = Visibility.Visible;
                        EmbedViewer.Children.Add(new EmbededInviteControl() { InviteCode = match.Groups[3].Value });
                    }
                }

                // Check if the Author is blocked
                if (LocalState.Blocked.ContainsKey(userid))
                {
                    IsBlocked = true;
                    content.Visibility = Visibility.Collapsed;
                    BlockedMessage.Visibility = Visibility.Visible;
                }
                else
                {
                    IsBlocked = false;
                    BlockedMessage.Visibility = Visibility.Collapsed;
                    content.Visibility = Visibility.Visible;
                }
            }
            // Clear message
            else
            {
                // Clear MessageId
                messageid = "";
                content.Visibility = Visibility.Visible;
                Grid.SetRow(moreButton, 2);
                username.Content = "";
                Avatar.Fill = null;
                timestamp.Text = "";
                content.Text = "";
                if (rootGrid.Children.Contains(reactionView))
                    rootGrid.Children.Remove(reactionView);
                reactionView = null;

                /* The resetting of the embed and attachement related stuff is handled by this function: */
                LoadEmbedsAndAttachements();
            }

            // Update content visibility
            if (string.IsNullOrEmpty(content.Text))
            {
                content.Visibility = Visibility.Collapsed;
                Grid.SetRow(moreButton, 3);
            }
            else
            {
                content.Visibility = Visibility.Visible;
                Grid.SetRow(moreButton, 2);
            }
        }

        /// <summary>
        /// Find all indexes of <paramref name="searchstring"/> in <paramref name="str"/>
        /// </summary>
        /// <param name="str">String to search in</param>
        /// <param name="searchstring">String to search for</param>
        /// <returns>List of indexes for <paramref name="searchstring"/></returns>
        public static IEnumerable<int> AllIndexesOf(string str, string searchstring)
        {
            if (str != null)
            {
                int minIndex = str.IndexOf(searchstring);
                while (minIndex != -1)
                {
                    yield return minIndex;
                    minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
                }
            }
        }

        ToggleButton reactionToggle;

        /// <summary>
        /// Generate ToggleButton for reaction
        /// </summary>
        /// <param name="reaction">Reaction to make button for</param>
        /// <returns>ToggleButton</returns>
        private ToggleButton GenerateReactionToggle(Reactions reaction)
        {
            // Make button
            reactionToggle = new ToggleButton();
            reactionToggle.IsChecked = reaction.Me;
            reactionToggle.Tag =
                new Tuple<string, string, Reactions>(Message.ChannelId, Message.Id, reaction);
            reactionToggle.Click += ToggleReaction;

            // Toggle CurrentUser reaction participation
            if (reaction.Me)
            {
                reactionToggle.IsChecked = true;
            }

            // Stack, reaction content
            StackPanel stack = new StackPanel() { Orientation = Orientation.Horizontal };

            // Get server emoji (if it's a server emoji)
            string serversideEmoji = null;
            if (!App.CurrentGuildIsDM)
            {
                if (LocalState.Guilds[App.CurrentGuildId].Raw.Emojis != null)
                    foreach (Emoji emoji in LocalState.Guilds[App.CurrentGuildId].Raw.Emojis)
                    {
                        if (emoji.Name == reaction.Emoji.Name)
                        {
                            serversideEmoji = emoji.Id;
                            if (emoji.Animated) serversideEmoji = serversideEmoji + ".gif";
                            else serversideEmoji = serversideEmoji + ".png";
                        }

                    }
            }
            
            // Display server emoji
            if (serversideEmoji != null)
            {
                stack.Children.Add(new Image()
                {
                    Width = 18,
                    Height = 18,
                    Source = new BitmapImage(new Uri("https://cdn.discordapp.com/emojis/" + serversideEmoji)),
                    VerticalAlignment = VerticalAlignment.Center
                });
            }
            else
            {
                string emoji = reaction.Emoji.Name;

                // Display standard emoji in Twitter emoji font
                if (NeoSmart.Unicode.Emoji.IsEmoji(emoji))
                {
                    stack.Children.Add(new TextBlock()
                    {
                        FontSize = 18,
                        Text = reaction.Emoji.Name,
                        FontFamily = new FontFamily("ms-appx:/Assets/emojifont.ttf#Twitter Color Emoji"),
                        VerticalAlignment = VerticalAlignment.Center
                    });
                }
                // Display Discord emoji
                else
                {
                    string extension = ".png";
                    if (reaction.Emoji.Animated) extension = ".gif";
                    stack.Children.Add(new Image()
                    {
                        Width = 18,
                        Height = 18,
                        Source = new BitmapImage(new Uri("https://cdn.discordapp.com/emojis/" + reaction.Emoji.Id + extension)),
                        VerticalAlignment = VerticalAlignment.Center
                    });
                }
            }

            // Add reaction count
            stack.Children.Add(new TextBlock() { Text = reaction.Count.ToString(), Margin = new Thickness(4, 0, 0, 0) });
            stack.Clip = new RectangleGeometry() { Rect = new Rect(0, -4, 96, 28) };
            reactionToggle.Content = stack;
            reactionToggle.Style = (Style)App.Current.Resources["EmojiButton"];
            reactionToggle.MinHeight = 0;
            reactionToggle.Height = 32;
            return reactionToggle;
        }

        /// <summary>
        /// Load or clear all Embeds and attachments
        /// </summary>
        private void LoadEmbedsAndAttachements()
        {
            // Clear Embeds
            EmbedViewer.Visibility = Visibility.Collapsed;
            EmbedViewer.Children.Clear();

            // If the Message is null return
            if (Message == null) return;

            // If there's any embeds or attachments show the EmbedViewer
            if (Message.Embeds.Any() || Message.Attachments.Any() || Message.Activity != null)
                EmbedViewer.Visibility = Visibility.Visible;

            // If there's any embeds
            if (Message.Embeds != null)
            {
                foreach (Embed embed in Message.Embeds)
                {
                    // Handle gifv
                    if (embed.Type == "gifv")
                    {
                        EmbedViewer.Children.Add(new GifvControl() { EmbedContent = embed });
                        // TODO: add attachement control instead of embed control
                    }
                    // Handle images
                    else if (embed.Type == "image")
                    {
                        EmbedViewer.Children.Add(new AttachementControl()
                        {
                            DisplayedAttachement = new Attachment()
                            {
                                Filename = "file.jpg",
                                Width = embed.Thumbnail.Width,
                                Height = embed.Thumbnail.Height,
                                Url = embed.Thumbnail.Url,
                                Size = 0
                            }
                        });
                    }
                    // Handle videos
                    else if (embed.Type == "video")
                    {
                        //TODO: Handle video differently
                        if (embed.Url.Contains("youtube"))
                        {
                            EmbedViewer.Children.Add(new VideoEmbedControl() { EmbedContent = embed });
                        }
                        else
                        {
                            EmbedViewer.Children.Add(new VideoEmbedControl() { EmbedContent = embed });
                        }
                    }
                    // Handle all else with EmbedControl
                    else
                    {
                        //The difference between rich content and article is done within the EmbedControl
                        EmbedViewer.Children.Add(new EmbedControl() { EmbedContent = embed });
                    }

                }
            }

            // If there's any attachments
            if (Message.Attachments != null)
            {
                foreach (Attachment attach in Message.Attachments)
                {
                    EmbedViewer.Children.Add(new AttachementControl() { DisplayedAttachement = attach });
                }
            }

            // Handle Activity
            if (Message.Activity != null)
            {
                // Spotify
                if (Message.Activity.Type == 3)
                {
                    // Show SpotifyShare Control
                    var spotifylisten = new ListenOnSpotify();
                    EmbedViewer.Children.Add(spotifylisten);
                    spotifylisten.Setup(Message.User.Id, Message.Activity.PartyId);
                }
            }
        }

        /// <summary>
        /// TODO: Determine if embed should display content without border
        /// </summary>
        private bool EmbedIsNoBorder(Embed embed)
        {
            return false;
        }

        /// <summary>
        /// Open MessageContext menu
        /// </summary>
        private void moreButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle delete button visibility in Guild
            if (App.CurrentGuildId != null)
            {
                if (!LocalState.Guilds[App.CurrentGuildId].channels[Message.ChannelId].permissions.ManageMessages)
                {
                    MoreDelete.Visibility = Visibility.Collapsed;
                }
            }

            // Override for CurrentUser message
            if (Message?.User.Id == LocalState.CurrentUser.Id)
            {
                MoreEdit.Visibility = Visibility.Visible;
                MoreReply.Visibility = Visibility.Collapsed;
                MoreDelete.Visibility = Visibility.Visible;
            }

            // Show Menu
            FlyoutBase.ShowAttachedFlyout(sender as Button);
        }

        /// <summary>
        /// Show Context Menu at right click point
        /// </summary>
        private void UserControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // If not an advert
            if (MessageType != MessageTypes.Advert)
            {
                // Handle delete button visibility in Guild
                if (App.CurrentGuildId != null && App.CurrentChannelId != null)
                {
                    if (!LocalState.CurrentGuildChannel.permissions.ManageMessages)
                    {
                        MoreDelete.Visibility = Visibility.Collapsed;
                        MorePin.Visibility = Visibility.Collapsed;
                    }
                }

                // Override for CurrentUser message
                if (Message?.User.Id == LocalState.CurrentUser.Id)
                {
                    MoreEdit.Visibility = Visibility.Visible;
                    MoreReply.Visibility = Visibility.Collapsed;
                    MoreDelete.Visibility = Visibility.Visible;
                }

                // Get display target element 
                UIElement tappedItem = (UIElement)e.OriginalSource;

                // Get menu to display
                MenuFlyout attachedFlyout = (MenuFlyout)FlyoutBase.GetAttachedFlyout(moreButton);

                // Display menu
                attachedFlyout.ShowAt(tappedItem, e.GetPosition(tappedItem));
            }
        }

        /// <summary>
        /// Show Content Menu at holding point
        /// </summary>
        private void UserControl_Holding(object sender, HoldingRoutedEventArgs e)
        {
            // If not an advert
            if (MessageType != MessageTypes.Advert)
            {
                // Handle delete button visibility in Guild
                if (App.CurrentGuildId != null)
                {
                    if (!LocalState.Guilds[App.CurrentGuildId].channels[Message.ChannelId].permissions.ManageMessages && !LocalState.Guilds[App.CurrentGuildId].channels[Message.ChannelId].permissions.Administrator && Message?.User.Id != LocalState.CurrentUser.Id && LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId != LocalState.CurrentUser.Id)
                    {
                        MoreDelete.Visibility = Visibility.Collapsed;
                    }
                }

                // Override for CurrentUser message
                if (Message?.User.Id == LocalState.CurrentUser.Id)
                {
                    MoreEdit.Visibility = Visibility.Visible;
                    MoreReply.Visibility = Visibility.Collapsed;
                }

                // Get display target element 
                UIElement tappedItem = (UIElement)e.OriginalSource;

                // Get menu to display
                MenuFlyout attachedFlyout = (MenuFlyout)FlyoutBase.GetAttachedFlyout(moreButton);

                // Display menu
                attachedFlyout.ShowAt(tappedItem, e.GetPosition(tappedItem));
            }
        }

        /// <summary>
        /// Toggle reaction on API
        /// </summary>
        private async void ToggleReaction(object sender, RoutedEventArgs e)
        {
            // Get reaction name
            var tuple = (sender as ToggleButton).Tag as Tuple<string, string, Reactions>;
            var reaction = tuple.Item3;
            string emojiStr = reaction.Emoji.Name;

            // Get emoji Id
            if (reaction.Emoji.Id != null)
                emojiStr += ":" + reaction.Emoji.Id;

            // Send Reaction Request
            if ((sender as ToggleButton)?.IsChecked == false) // Inverted since it changed
            {
                // Delete reaction
                await RESTCalls.DeleteReactionAsync(tuple.Item1, tuple.Item2, emojiStr);
            }
            else
            {
                // Add reaction
                await RESTCalls.CreateReactionAsync(tuple.Item1, tuple.Item2, emojiStr);
            }
        }

        /// <summary>
        /// Value typed for edit
        /// </summary>
        string EditValue = "";

        /// <summary>
        /// MessageBox object used for editing message
        /// </summary>
        MessageBox editBox;

        /// <summary>
        /// Open edit UI
        /// </summary>
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // Initialize editBox
            if (EditValue.Trim() == "") EditValue = content.Text;
            editBox = new MessageBox()
            {
                Text = EditValue.Trim(),
                Background = new SolidColorBrush(Colors.Transparent),
                Padding = new Thickness(-14, 4, 0, 6),
                FontSize = 14,
                IsEdit = true
            };
            editBox.Send += EditBox_Send;
            editBox.Cancel += EditBox_Cancel;
            editBox.TextChanged += EditBox_TextChanged;
            //editBox.LostFocus += EditBox_Cancel;
            Grid.SetRow(editBox, 2);
            Grid.SetColumn(editBox, 1);

            // Show editBox
            rootGrid.Children.Add(editBox);

            // Hide content
            content.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Cancel EditBox
        /// </summary>
        private void EditBox_Cancel(object sender, RoutedEventArgs e)
        {
            // Dispose EditBox
            editBox.Send -= EditBox_Send;
            editBox.Cancel -= EditBox_Cancel;
            editBox.TextChanged -= EditBox_TextChanged;
            editBox.LostFocus -= EditBox_Cancel;

            // Hide EditBox
            rootGrid.Children.Remove(editBox);

            // Show content
            content.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Update EditValue
        /// </summary>
        private void EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update EditValue
            EditValue = editBox.Text;
        }

        /// <summary>
        /// Edit message on API
        /// </summary>
        private async void EditBox_Send(object sender, RoutedEventArgs e)
        {
            // Disable edit box
            editBox.IsEnabled = false;

            // Edit value
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await RESTCalls.EditMessageAsync(Message.ChannelId, Message.Id, editBox.Text));

            // Dispose EditBox
            editBox.Send -= EditBox_Send;
            editBox.Cancel -= EditBox_Cancel;
            editBox.TextChanged -= EditBox_TextChanged;
            editBox.LostFocus -= EditBox_Cancel;

            // Hide editBox
            rootGrid.Children.Remove(editBox);

            // Show content
            content.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Update bin status
        /// </summary>
        private async void MorePin_Click(object sender, RoutedEventArgs e)
        {
            if (Message.Pinned)
            {
                // Unpin message
                await RESTCalls.UnpinMessage(Message.ChannelId, Message.Id);
            }
            else
            {
                // Pin message
                await RESTCalls.PinMessage(Message.ChannelId, Message.Id);
            }
        }

        /// <summary>
        /// Delete Message
        /// </summary>
        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            // Create app prompt to delete message
            App.DeleteMessage(Message.ChannelId, Message.Id);
        }

        /// <summary>
        /// Copy ID to clipboard
        /// </summary>
        private void MoreCopyId_Click(object sender, RoutedEventArgs e)
        {
            // Copy ID to clipboard
            var dataPackage = new DataPackage();
            dataPackage.SetText(Message.Id);
            Clipboard.SetContent(dataPackage);
        }

        /// <summary>
        /// Show MemberFlyout
        /// </summary>
        private void Username_OnClick(object sender, RoutedEventArgs e)
        {
            // Prompt app for MemberFlyout
            App.ShowMemberFlyout(username, Message.User, Message.WebHookid != null);
        }

        /// <summary>
        /// Show User Context Menu on right-click
        /// </summary>
        private void username_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (!App.CurrentGuildIsDM)
                    // Prompt app for flyout
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, Message.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        /// <summary>
        /// Show User Context Menu on holding
        /// </summary>
        private void username_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                if (!App.CurrentGuildIsDM)
                    // Prompt app for flyout
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, Message.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        /// <summary>
        /// Flyout containing emojiPicker
        /// </summary>
        Flyout PickReaction;

        /// <summary>
        /// emoji Picker for selecting reaction
        /// </summary>
        EmojiControl emojiPicker;

        /// <summary>
        /// Show emoji picker
        /// </summary>
        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            // Initalize pickers
            PickReaction = new Flyout();
            emojiPicker = new EmojiControl();
            emojiPicker.PickedEmoji += ReactionSelected;
            PickReaction.FlyoutPresenterStyle = (Style)App.Current.Resources["FlyoutPresenterStyle1"];
            PickReaction.Content = emojiPicker;

            // Show picker
            PickReaction.ShowAt(moreButton);
        }

        /// <summary>
        /// When an emoji is selected from emojiPicker
        /// </summary>
        private async void ReactionSelected(object sender, EmojiControl.ISimpleEmoji e)
        {
            // Hide control if not holding shift
            if (!CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                PickReaction.Hide();

            // Get emoji as string
            string emojiStr = e.surrogates;
            if (e.GetType() == typeof(EmojiControl.GuildSide))
            {
                var emoji = (EmojiControl.GuildSide)e;
                emojiStr = emoji.names[0] + ":" + emoji.id;
            }

            // Create reaction
            await RESTCalls.CreateReactionAsync(Message.ChannelId, messageid, emojiStr);
        }

        /// <summary>
        /// Add reply to draft
        /// </summary>
        private void MoreReply_Click(object sender, RoutedEventArgs e)
        {
            // Prompt app to add reply to draft
            App.MentionUser(Message.User.Username, Message.User.Discriminator);
        }

        /// <summary>
        /// Inform SideDrawer of Content Press
        /// </summary>
        private void UserControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            App.UniversalPointerDown(e);
        }

        /// <summary>
        /// Show blocked message
        /// </summary>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            IsBlocked = false;
            // Show content
            content.Visibility = Visibility.Visible;

            // Hide blocked cover
            BlockedMessage.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            Debug.WriteLine("Disposed of messagecontrol");
            GatewayManager.Gateway.MessageReactionAdded -= GatewayOnMessageReactionAdded;
            GatewayManager.Gateway.MessageReactionRemoved -= GatewayOnMessageReactionRemoved;
            if (emojiPicker != null)
                emojiPicker.PickedEmoji -= ReactionSelected;
            if (reactionToggle != null)
                reactionToggle.Click -= ToggleReaction;
            if (editBox != null)
            {
                editBox.Send -= EditBox_Send;
                editBox.Cancel -= EditBox_Cancel;
                editBox.TextChanged -= EditBox_TextChanged;
            }
        }

        /// <summary>
        /// Dispose on unloading
        /// </summary>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
    }
}
