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
using static Discord_UWP.Common;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Media.Animation;
using Discord_UWP.Controls;
using Discord_UWP.Gateway;
using Discord_UWP.Gateway.DownstreamEvents;
using Discord_UWP.SharedModels;

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using static Discord_UWP.Managers.MessageManager;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Discord_UWP.SimpleClasses;
using Windows.System;

namespace Discord_UWP.Controls
{
    public sealed partial class MessageControl : UserControl
    {

        //Is the more button visible?
        public Visibility MoreButtonVisibility
        {
            get { return moreButton.Visibility; }
            set { moreButton.Visibility = value; }
        }

        //Is the message an advert?
        
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

        //Is the message the continuation of another one?
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

        //The header of the messages, that can indicate data such as "new messages" or the date
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

        //The message to be displayed
        public SharedModels.Message Message
        {
            get { return (SharedModels.Message)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message),
            typeof(SharedModels.Message),
            typeof(MessageControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));


        //Calls OnPropertyChanged for this instance of the control
        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as MessageControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        AdControl advert;
        string originalcontent = "";
        /* IT IS VERY IMPORTANT TO REMEMBER THE MESSAGECONTROL GET RECYCLED BY VIRTUALIZATION, AND THAT VALUES MUST SYSTEMATICALLY BE RESET
         * For example, if you change a color depending on a boolean property, make sure to include a `else` that will reset the color */
        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == IsContinuationProperty)
            {
                if (IsContinuation)
                    VisualStateManager.GoToState(((MessageControl)d), "Continuation", false);
                else
                    VisualStateManager.GoToState(((MessageControl)d), "VisualState", false);
            }
            if(Storage.Settings.CompactMode)
                VisualStateManager.GoToState(((MessageControl)d), "Compact", false);

            if (prop == MessageTypeProperty)
            {
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
                    if(reactionView!=null)
                    rootGrid.Children.Remove(reactionView);
                    return;
                } else
                {
                    if (rootGrid.Children.Contains(advert))
                        rootGrid.Children.Remove(advert);
                    advert = null;

                    if (MessageType == MessageTypes.RecipientAdded)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["online"]);
                        content.Text = "**" + Message.User.Username + "** " + App.GetString("/Controls/AddedUser") + App.GetString("/Controls/ToTheConversation").Replace("<user>", "**" + Message.Mentions.First().Username + "**");
                    }
                    else if (MessageType == MessageTypes.RecipientRemoved)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["dnd"]);
                        content.Text = "**" + Message.User.Username + "** " + App.GetString("/Controls/RemovedUser") + App.GetString("/Controls/FromTheConversation").Replace("<user>", "**" + Message.Mentions.First().Username + "**");
                    }
                    else if(MessageType == MessageTypes.ChannelIconChanged)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["InvertedBG"]);
                        content.Text = "**" + Message.User.Username + "** changed the channel's icon"; 
                    }
                    else if (MessageType == MessageTypes.ChannelNameChanged)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush)App.Current.Resources["InvertedBG"]);
                        content.Text = "**" + Message.User.Username + "** changed the channel's name";
                    }
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

                    else if (MessageType == MessageTypes.GuildMemberJoined)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        SetAltIcon("", (SolidColorBrush) App.Current.Resources["online"]);
                        content.Text = "**"+Message.User.Username + "**" + " joined the server!";
                    }
                    else if(MessageType == MessageTypes.ChannelIconChanged)
                    {

                    }
                    else if (MessageType == MessageTypes.Default)
                    {
                        if (IsContinuation)
                            VisualStateManager.GoToState(((MessageControl)d), "Continuation", false);
                        else
                            VisualStateManager.GoToState(((MessageControl)d), "VisualState", false);
                    }
                }
            }
            if (prop == MessageProperty)
            {
                UpdateMessage(Edited);
            }
            if (prop == LastReadProperty)
            {
                if (LastRead)
                {
                    HeaderUI.Visibility = Visibility.Visible;
                } else
                {
                    HeaderUI.Visibility = Visibility.Collapsed;
                }
            }
            //if (prop == HeaderProperty)
            //{
            //    if (Header != null && Header != "null")
            //    {
            //        HeaderUI.Visibility = Visibility.Visible;
            //        HeaderText.Text = Header;
            //    } else
            //    {
            //        HeaderUI.Visibility = Visibility.Collapsed;
            //    }
            //}
            if (prop == IsPendingProperty)
            {
                if (IsPending)
                {
                    content.Opacity = 0.5;
                } else
                {
                    content.Opacity = 1;
                }
            }
            if (prop == IsBlockedProperty)
            {
                if (IsBlocked)
                {
                    content.Visibility = Visibility.Collapsed;
                    BlockedMessage.Visibility = Visibility.Visible;
                } else
                {
                    content.Visibility = Visibility.Visible;
                    BlockedMessage.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public MessageControl()
        {
            this.InitializeComponent();
            if (GatewayManager.Gateway != null) //idrk
            {
                GatewayManager.Gateway.MessageReactionAdded += GatewayOnMessageReactionAdded;
                GatewayManager.Gateway.MessageReactionRemoved += GatewayOnMessageReactionRemoved;
            }
        }

        private void SetAltIcon(string glyph, SolidColorBrush color, int fontsize = 18, bool mirrored = false)
        {
            AlternativeIcon.Glyph = glyph;
            AlternativeIcon.FontSize = fontsize;
            AlternativeIcon.Foreground = color;
            AlternativeIcon.RenderTransform = mirrored ? new ScaleTransform(){ ScaleX = -1, CenterX = 9} : new ScaleTransform() { ScaleX = 1 };
        }

        private async void GatewayOnMessageReactionRemoved(object sender, GatewayEventArgs<MessageReactionUpdate> gatewayEventArgs)
        {
            if (gatewayEventArgs.EventData.MessageId != messageid) return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    if (reactionView == null)
                        reactionView = GenerateWrapGrid();
                    ToggleButton toRemove = null;
                    foreach (ToggleButton toggle in reactionView.Children)
                    {
                        var tuple = toggle.Tag as Tuple<string, string, Reactions>;
                        if (tuple.Item3.Emoji.Name == gatewayEventArgs.EventData.Emoji.Name)
                        {
                            var tb = ((toggle.Content as StackPanel).Children.Last() as TextBlock);
                            var text = tb.Text;
                            //var rt = ((toggle.Content as StackPanel).Children.Last() as TextBlock).RenderTransform = new TranslateTransform();
                            if (text == "1")
                            {
                                toRemove = toggle;
                                break;
                            }
                            if (tuple.Item3.Me)
                                toggle.IsChecked = false;
                            AnimationSet.UseComposition = true;
                            await tb.Offset(22, -18, 150, 0, EasingType.Back, EasingMode.EaseIn).StartAsync();
                            tb.Text = (Convert.ToInt32(text) - 1).ToString();

                            await tb.Offset(22, 18, 0).StartAsync();
                            await tb.Offset(22, 0, 180, 0, EasingType.Back, EasingMode.EaseOut).StartAsync();
                            AnimationSet.UseComposition = false;
                        }
                    }
                    if (toRemove != null)
                        reactionView.Children.Remove(toRemove);
                });
        }

        private async void GatewayOnMessageReactionAdded(object sender, GatewayEventArgs<MessageReactionUpdate> gatewayEventArgs)
        {
            if (gatewayEventArgs.EventData.MessageId != messageid) return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    if (reactionView == null)
                    {
                        reactionView = GenerateWrapGrid();
                        rootGrid.Children.Add(reactionView);
                    }


                    bool success = false;
                    foreach (ToggleButton toggle in reactionView.Children)
                    {
                        var tuple = toggle.Tag as Tuple<string, string, Reactions>;
                        if (tuple.Item3.Emoji.Name == gatewayEventArgs.EventData.Emoji.Name)
                        {
                            success = true;
                            var tb = ((toggle.Content as StackPanel).Children.Last() as TextBlock);
                            var text = tb.Text;
                            //animate from the top
                           // var rt = tb.RenderTransform = new TranslateTransform();
                            if (tuple.Item3.Me)
                                toggle.IsChecked = true;
                            AnimationSet.UseComposition = true;
                            await tb.Offset(22, 18, 180, 0, EasingType.Back, EasingMode.EaseIn).StartAsync();
                            tb.Text = (Convert.ToInt32(text) + 1).ToString();
                            
                            await tb.Offset(22, -18, 0).StartAsync();
                            await tb.Offset(22, 0, 180, 0, EasingType.Back, EasingMode.EaseOut).StartAsync();
                            AnimationSet.UseComposition = false;
                        }
                    }
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

        private void Sb_Completed(object sender, object e)
        {
            
        }

        private WrapPanel reactionView;
        //private Attachment attachement;
        private string userid = "";
        public string messageid = "";
        public Microsoft.Toolkit.Uwp.UI.Controls.WrapPanel GenerateWrapGrid()
        {
            var wg = new WrapPanel()
            {
                Orientation=Orientation.Horizontal,
                Margin = new Thickness(6, 4, 0, 0),
                HorizontalSpacing=4,
                VerticalSpacing=4
            };
            return wg;
            
        }
        public void UpdateMessage(bool edited = false)
        {
            content.FontSize = Storage.Settings.MSGFontSize;
            content.Padding = new Thickness(5, 3, 3, content.FontSize / 4 + 1);

            if (rootGrid.Children.Contains(advert))
                rootGrid.Children.Remove(advert);
            advert = null;
            
            if (!edited && rootGrid.Children.Contains(reactionView))
                rootGrid.Children.Remove(reactionView);
            if (Message != null)
            {
                messageid = Message.Id;
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

                if (Message.User == null) return;
                if (Message.User.Username != null)
                    username.Content = Message.User.Username;
                else
                    username.Content = "";
                GuildMember member;
                if (Message.User.Id != null) userid = Message.User.Id;
                else userid = "";
                if (App.CurrentGuildId != null && Message.User.Id != null)
                {
                    if (LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(Message.User.Id))
                    {
                        member = LocalState.Guilds[App.CurrentGuildId].members[Message.User.Id];
                    } else
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

                if (Message.Activity != null)
                {
                    if(Message.Activity.Type == 3)
                    {
                        //Spotify
                        
                    }
                }

                //if (member.User.Id != null && LocalState.PresenceDict.ContainsKey(member.User.Id))
                //{
                //    if (LocalState.PresenceDict[member.User.Id].Status != null && LocalState.PresenceDict[member.User.Id].Status != "invisible")
                //        ShadowPresence.Color = (Color)App.Current.Resources[Common.Capitalize(LocalState.PresenceDict[member.User.Id].Status) + "Color"];
                //    else if (LocalState.PresenceDict[member.User.Id].Status == "invisible")
                //        //ShadowPresence.Color = (Color)App.Current.Resources["OfflineColor"];
                //        ShadowPresence.Visibility = Visibility.Collapsed;
                //}
                //else
                //{
                //    //ShadowPresence.Color = (Color)App.Current.Resources["OfflineColor"];
                //    ShadowPresence.Visibility = Visibility.Collapsed;
                //}

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

                if (Message.User.Bot == true)
                    BotIndicator.Visibility = Visibility.Visible;
                else
                    BotIndicator.Visibility = Visibility.Collapsed;
                if (Message.Pinned)
                    MorePin.Text = App.GetString("/Controls/Unpin");
                else
                    MorePin.Text = App.GetString("/Controls/Pin") + " ";

                if (!Storage.Settings.DevMode)
                    MoreCopyId.Visibility = Visibility.Collapsed;

                AvatarBrush.ImageSource = new BitmapImage(Common.AvatarUri(Message.User.Avatar, Message.User.Id));

                if (Message.User.Avatar == null)
                    AvatarBG.Fill = Common.DiscriminatorColor(Message.User.Discriminator);
                else
                    AvatarBG.Fill = Common.GetSolidColorBrush("#00000000");

                timestamp.Text = Common.HumanizeDate(Message.Timestamp, null);
                if (Message.EditedTimestamp.HasValue)
                    timestamp.Text += " (" + App.GetString("/Controls/Edited") + " " + Common.HumanizeEditedDate(Message.EditedTimestamp.Value,
                                          Message.Timestamp) + ")";

                
                LoadEmbedsAndAttachements();

                if (!edited)
                {
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

                content.Users = Message.Mentions;
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
                content.Text = Message.Content;
                Regex regex = new Regex("(discord\\.(gg|io|me|li)\\/|discordapp\\.com\\/invite\\/)(([A-Za-z]|[0-9])+)");
                foreach (Match match in regex.Matches(content.Text))
                {
                    if (match.Groups.Count >= 3)
                    {
                        EmbedViewer.Visibility = Visibility.Visible;
                        EmbedViewer.Children.Add(new EmbededInviteControl(){ InviteCode=match.Groups[3].Value });
                    }
                }
                //string startLink = "";


                if (LocalState.Blocked.ContainsKey(userid))
                {
                    IsBlocked = true;
                    content.Visibility = Visibility.Collapsed;
                    BlockedMessage.Visibility = Visibility.Visible;
                } else
                {
                    IsBlocked = false;
                    BlockedMessage.Visibility = Visibility.Collapsed;
                    content.Visibility = Visibility.Visible;
                }
            }
            else
            {
                messageid = "";
                content.Visibility = Visibility.Visible;
                Grid.SetRow(moreButton,2);
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
        }
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
        private ToggleButton GenerateReactionToggle(Reactions reaction)
        {
            reactionToggle = new ToggleButton();
            reactionToggle.IsChecked = reaction.Me;
            reactionToggle.Tag =
                new Tuple<string, string, Reactions>(Message.ChannelId, Message.Id, reaction);
            reactionToggle.Click += ToggleReaction;
            if (reaction.Me)
            {
                reactionToggle.IsChecked = true;
            }
            StackPanel stack = new StackPanel() { Orientation = Orientation.Horizontal };
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
            if (serversideEmoji != null)
            {
                stack.Children.Add(new Windows.UI.Xaml.Controls.Image()
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
                else
                {
                    string extension = ".png";
                    if (reaction.Emoji.Animated) extension = ".gif";
                    stack.Children.Add(new Windows.UI.Xaml.Controls.Image()
                    {
                        Width = 18,
                        Height = 18,
                        Source = new BitmapImage(new Uri("https://cdn.discordapp.com/emojis/" + reaction.Emoji.Id + extension)),
                        VerticalAlignment = VerticalAlignment.Center
                    });
                }
            }
            stack.Children.Add(new TextBlock() { Text = reaction.Count.ToString(), Margin = new Thickness(4, 0, 0, 0) });
            stack.Clip = new RectangleGeometry(){Rect=new Rect(0,-4,96,28)};
            reactionToggle.Content = stack;
            reactionToggle.Style = (Style)App.Current.Resources["EmojiButton"];
            reactionToggle.MinHeight = 0;
            reactionToggle.Height = 32;
            return reactionToggle;
        }
        private void LoadEmbedsAndAttachements()
        {
            EmbedViewer.Visibility = Visibility.Collapsed;
            EmbedViewer.Children.Clear();

            if (Message == null) return;
            if (Message.Embeds.Any() || Message.Attachments.Any() || Message.Activity != null)
                EmbedViewer.Visibility = Visibility.Visible;
            
            if (Message.Embeds != null)
            {
                foreach (Embed embed in Message.Embeds)
                {
                    if (embed.Type == "gifv")
                    {
                        EmbedViewer.Children.Add(new GifvControl() { EmbedContent = embed });
                        //TODO add attachement control instead of embed control
                    }
                    else if(embed.Type == "image")
                    {
                        EmbedViewer.Children.Add(new AttachementControl() { DisplayedAttachement = new Attachment()
                        {
                            Filename = "file.jpg",
                            Width = embed.Thumbnail.Width,
                            Height = embed.Thumbnail.Height,
                            Url = embed.Thumbnail.Url,
                            Size = 0
                        }
                        });
                    }
                    else if(embed.Type == "video")
                    {
                        //TODO: Handle video differently
                        if (embed.Url.Contains("youtube"))
                        {
                            EmbedViewer.Children.Add(new VideoEmbedControl() { EmbedContent = embed });
                        } else
                        {
                            EmbedViewer.Children.Add(new VideoEmbedControl() { EmbedContent = embed });
                        }
                    }
                    else
                    {
                        //The difference between rich content and article is done within the EmbedControl
                        EmbedViewer.Children.Add(new EmbedControl() { EmbedContent = embed });
                    }
                    
                }
            }
            if (Message.Attachments != null)
            {
                foreach (Attachment attach in Message.Attachments)
                {
                    EmbedViewer.Children.Add(new AttachementControl() { DisplayedAttachement = attach });
                }
            }
            if (Message.Activity != null)
            {
                if(Message.Activity.Type == 3)
                {
                    var spotifylisten = new ListenOnSpotify();
                    EmbedViewer.Children.Add(spotifylisten);
                    spotifylisten.Setup(Message.User.Id, Message.Activity.PartyId);
                }
            }
        }
        private bool EmbedIsNoBorder(Embed embed)
        {
            return false;
        }

        private void moreButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentGuildId != null)
            {
                if (!LocalState.Guilds[App.CurrentGuildId].channels[Message.ChannelId].permissions.ManageMessages && !LocalState.Guilds[App.CurrentGuildId].channels[Message.ChannelId].permissions.Administrator && Message?.User.Id != LocalState.CurrentUser.Id && LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId != LocalState.CurrentUser.Id)
                {
                    MoreDelete.Visibility = Visibility.Collapsed;
                }
            }

            if (Message?.User.Id == LocalState.CurrentUser.Id)
            {
                MoreEdit.Visibility = Visibility.Visible;
                MoreReply.Visibility = Visibility.Collapsed;
            }
            //if (Storage.Settings.savedMessages.ContainsKey(messageid))
            //{
            //    MoreSave.Text = "Unsave"; //TODO:Translate
            //}
            FlyoutBase.ShowAttachedFlyout(sender as Button);
        }

        private void UserControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (MessageType != MessageTypes.Advert)
            {

                if (App.CurrentGuildId != null)
                {
                    if (!LocalState.Guilds[App.CurrentGuildId].channels[Message.ChannelId].permissions.ManageMessages && !LocalState.Guilds[App.CurrentGuildId].channels[Message.ChannelId].permissions.Administrator && Message?.User.Id != LocalState.CurrentUser.Id && LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId != LocalState.CurrentUser.Id)
                    {
                        MoreDelete.Visibility = Visibility.Collapsed;
                    }
                }

                if (Message?.User.Id == LocalState.CurrentUser.Id)
                {
                    MoreEdit.Visibility = Visibility.Visible;
                    MoreReply.Visibility = Visibility.Collapsed;
                }
                FlyoutBase.ShowAttachedFlyout(moreButton);
            }
        }

        private void UserControl_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (MessageType != MessageTypes.Advert)
            {
                if (App.CurrentGuildId != null)
                {
                    if (!LocalState.Guilds[App.CurrentGuildId].channels[Message.ChannelId].permissions.ManageMessages && !LocalState.Guilds[App.CurrentGuildId].channels[Message.ChannelId].permissions.Administrator && Message?.User.Id != LocalState.CurrentUser.Id && LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId != LocalState.CurrentUser.Id)
                    {
                        MoreDelete.Visibility = Visibility.Collapsed;
                    }
                }
                if (Message?.User.Id == LocalState.CurrentUser.Id)
                {
                    MoreEdit.Visibility = Visibility.Visible;
                    MoreReply.Visibility = Visibility.Collapsed;
                }
                FlyoutBase.ShowAttachedFlyout(moreButton);
            }
        }

        private async void ToggleReaction(object sender, RoutedEventArgs e)
        {
            var counter = ((StackPanel) ((ToggleButton) sender).Content).Children.Last() as TextBlock;
            var tuple = (sender as ToggleButton).Tag as Tuple<string, string, Reactions>;
            var reaction = tuple.Item3;
            string emojiStr = reaction.Emoji.Name;
            if (reaction.Emoji.Id != null)
                emojiStr += ":" + reaction.Emoji.Id;
            if ((sender as ToggleButton)?.IsChecked == false) //Inverted since it changed
                await RESTCalls.DeleteReactionAsync(tuple.Item1, tuple.Item2, emojiStr);
            else
                await RESTCalls.CreateReactionAsync(tuple.Item1, tuple.Item2, emojiStr);
    }

        string EditValue = "";
        MessageBox editBox;
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (EditValue.Trim() == "") EditValue = content.Text;
            editBox = new MessageBox()
            {
                Text = EditValue.Trim(),
                Background = new SolidColorBrush(Windows.UI.Colors.Transparent),
                Padding = new Thickness(-14, 4, 0, 6),
                FontSize=14,
                IsEdit = true
            };
            editBox.Send += EditBox_Send;
            editBox.Cancel += EditBox_Cancel;
            editBox.TextChanged += EditBox_TextChanged;
            //editBox.LostFocus += EditBox_Cancel;
            Grid.SetRow(editBox, 2);
            Grid.SetColumn(editBox, 1);
            rootGrid.Children.Add(editBox);
            content.Visibility = Visibility.Collapsed;
        }

        private void EditBox_Cancel(object sender, RoutedEventArgs e)
        {
            editBox.Send -= EditBox_Send;
            editBox.Cancel -= EditBox_Cancel;
            editBox.TextChanged -= EditBox_TextChanged;
            editBox.LostFocus -= EditBox_Cancel;
            rootGrid.Children.Remove(editBox);
            content.Visibility = Visibility.Visible;
        }

        private void EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EditValue = editBox.Text;
        }

        private async void EditBox_Send(object sender, RoutedEventArgs e)
        {
            editBox.IsEnabled = false;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await RESTCalls.EditMessageAsync(Message.ChannelId, Message.Id, editBox.Text));
            editBox.Send -= EditBox_Send;
            editBox.Cancel -= EditBox_Cancel;
            editBox.TextChanged -= EditBox_TextChanged;
            editBox.LostFocus -= EditBox_Cancel;
            rootGrid.Children.Remove(editBox);
            content.Visibility = Visibility.Visible;
        }

        private async void MorePin_Click(object sender, RoutedEventArgs e)
        {
            if (Message.Pinned)
            {
                await RESTCalls.UnpinMessage(Message.ChannelId, Message.Id);
            } else
            {
                await RESTCalls.PinMesage(Message.ChannelId, Message.Id);
            }
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            App.DeleteMessage(Message.ChannelId, Message.Id);
        }

        private void MoreCopyId_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(Message.Id);
            Clipboard.SetContent(dataPackage);
        }

        private void Username_OnClick(object sender, RoutedEventArgs e)
        {
            App.ShowMemberFlyout(username, Message.User, Message.WebHookid!=null);
        }

        private void username_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (!App.CurrentGuildIsDM)
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, Message.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        private void username_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                if (!App.CurrentGuildIsDM)
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, Message.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        Flyout PickReaction;
        EmojiControl emojiPicker;
        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            PickReaction = new Flyout();
            EmojiControl emojiPicker = new EmojiControl();
            emojiPicker.PickedEmoji += ReactionSelected;
            PickReaction.FlyoutPresenterStyle = (Style)App.Current.Resources["FlyoutPresenterStyle1"];
            PickReaction.Content = emojiPicker;
            PickReaction.ShowAt(moreButton);
        }

        private async void ReactionSelected(object sender, EmojiControl.ISimpleEmoji e)
        {
            if(!CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                PickReaction.Hide();
            string emojiStr = e.surrogates;
            if(e.GetType() == typeof(EmojiControl.GuildSide))
            {
                var emoji = (EmojiControl.GuildSide)e;
                emojiStr = emoji.names[0] + ":" + emoji.id;
            }
            await RESTCalls.CreateReactionAsync(Message.ChannelId, messageid, emojiStr);
        }

        private void MoreReply_Click(object sender, RoutedEventArgs e)
        {
            App.MentionUser(Message.User.Username, Message.User.Discriminator);
        }

        private void UserControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            App.UniversalPointerDown(e);
        }

        private void contentStacker_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            IsBlocked = false;
            content.Visibility = Visibility.Visible;
            BlockedMessage.Visibility = Visibility.Collapsed;
        }

        private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
       //     if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse && moreButton.Opacity!=1 && ShowMoreButton.GetCurrentState() == ClockState.Stopped)
       //         ShowMoreButton.Begin();
            
        }

        private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
     //       if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse && moreButton.Opacity != 0 && HideMoreButton.GetCurrentState() == ClockState.Stopped)
       //         HideMoreButton.Begin();
        }

        //private void MoreSave_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Storage.Settings.savedMessages.ContainsKey(messageid))
        //    {
        //        Storage.Settings.savedMessages.Remove(messageid);
        //    }
        //    else
        //    {
        //        Storage.Settings.savedMessages.Add(messageid, Message.Value);
        //    }
        //}

        public void Dispose()
        {
            Debug.WriteLine("Disposed of messagecontrol");
            GatewayManager.Gateway.MessageReactionAdded -= GatewayOnMessageReactionAdded;
            GatewayManager.Gateway.MessageReactionRemoved -= GatewayOnMessageReactionRemoved;
            if(emojiPicker!= null)
                emojiPicker.PickedEmoji -= ReactionSelected;
            if(reactionToggle!=null)
                reactionToggle.Click -= ToggleReaction;
            if (editBox != null)
            {
                editBox.Send -= EditBox_Send;
                editBox.Cancel -= EditBox_Cancel;
                editBox.TextChanged -= EditBox_TextChanged;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
    }
}
