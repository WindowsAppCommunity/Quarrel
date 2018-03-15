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
        
        public MessageManager.MessageTypes MessageType
        {
            get { return (MessageManager.MessageTypes)GetValue(MessageTypeProperty); }
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

        //The message to be displayed
        public SharedModels.Message? Message
        {
            get { return (SharedModels.Message?)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message),
            typeof(SharedModels.Message?),
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
                    advert.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                    advert.AdUnitId = "336795";
                    advert.Margin = new Thickness(6);
                    advert.Background = new SolidColorBrush(Colors.Transparent);
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
                        AlternativeIcon.Glyph = "";
                        AlternativeIcon.Foreground = (SolidColorBrush)App.Current.Resources["online"];
                        content.Text = "**" + Message.Value.User.Username + "** " + App.GetString("/Controls/AddedUser") + " **" + Message.Value.Mentions.First().Username + "** " + App.GetString("/Controls/ToTheConversation");
                    }
                    else if (MessageType == MessageTypes.RecipientRemoved)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        AlternativeIcon.Glyph = "";
                        AlternativeIcon.Foreground = (SolidColorBrush)App.Current.Resources["dnd"];
                        content.Text = "**" + Message.Value.User.Username + "** " + App.GetString("/Controls/RemovedUser") + " **" + Message.Value.Mentions.First().Username + "** " + App.GetString("/Controls/FromTheConversation");
                    }
                    else if (MessageType == MessageTypes.Call)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        AlternativeIcon.Glyph = "";
                        AlternativeIcon.FontSize = 18;
                        AlternativeIcon.Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"];
                        //content.Text = App.GetString("/Controls/YouMissedACall") + " **" + Message.Value.User.Username + "**";
                        if (Message.Value.User.Id == LocalState.CurrentUser.Id)
                            content.Text = App.GetString("/Controls/You") + " " + App.GetString("/Controls/StartedACall");
                        else
                            content.Text = App.GetString("/Controls/CallStartedBy") + " **" + Message.Value.User.Username + "**";
                    }
                    else if (MessageType == MessageTypes.PinnedMessage)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        if (rootGrid.Children.Contains(advert))
                            rootGrid.Children.Remove(advert);
                        advert = null;
                        VisualStateManager.GoToState(this, "Alternative", false);
                        AlternativeIcon.Glyph = "";
                        AlternativeIcon.FontSize = 18;
                        AlternativeIcon.Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"];
                        content.Text = "**" + Message.Value.User.Username + "** " + App.GetString("/Controls/PinnedAMessageInThisChannel");
                    }

                    else if (MessageType == MessageTypes.GuildMemberJoined)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        AlternativeIcon.Glyph = "";
                        AlternativeIcon.FontSize = 18;
                        AlternativeIcon.Foreground = (SolidColorBrush)App.Current.Resources["online"];
                        content.Text = "**"+Message.Value.User.Username + "**" + " joined the server!";
                    }

                    
                    else if (MessageType == MessageTypes.FailedEncryption)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative", false);
                        AlternativeIcon.Glyph = "";
                        AlternativeIcon.FontSize = 18;
                        AlternativeIcon.Foreground = (SolidColorBrush)App.Current.Resources["dnd"];
                        content.Text = "There was an error while decrypting this message";
                    }
                    else if (MessageType == MessageTypes.StartEncryption)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative2", false);
                        AlternativeIcon.Glyph = "";
                        AlternativeIcon.Foreground = (SolidColorBrush)App.Current.Resources["idle"];
                        content.Text = "**" + Message.Value.User.Username + "** " + " wants this conversation to be encrypted";
                        var acceptButton = new HyperlinkButton()
                        {
                            Content = "Accept",
                            Foreground = (SolidColorBrush)App.Current.Resources["idle"],
                            Style = (Style)App.Current.Resources["PlainHyperlinkStyle"],
                        };
                        acceptButton.Click += AcceptButton_Click;
                        moreButton.Click += MoreButton_Click;
                        var moreinfoButton = new HyperlinkButton()
                        {
                            Content = "More info",
                            Foreground = (SolidColorBrush)App.Current.Resources["Greyple"],
                            Style = (Style)App.Current.Resources["PlainHyperlinkStyle"],
                            Margin = new Thickness(6, 0, 0, 0)
                        };
                        EmbedViewer.Children.Add(new StackPanel()
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(44, 0, 0, 0),
                            Children = { acceptButton, moreinfoButton }
                        });
                        EmbedViewer.Visibility = Visibility.Visible;
                    }
                    else if (MessageType == MessageTypes.AcceptEncryption)
                    {
                        if (rootGrid.Children.Contains(reactionView))
                            rootGrid.Children.Remove(reactionView);
                        VisualStateManager.GoToState(this, "Alternative2", false);
                        AlternativeIcon.Glyph = "";
                        AlternativeIcon.Foreground = (SolidColorBrush)App.Current.Resources["online"];
                        content.Text = "*This conversation is now end-to-end encrypted*";
                    }
                    else if (MessageType == MessageTypes.EncryptedMessage)
                    {
                        encrypted.Visibility = Visibility.Visible;
                        content.Text = EncryptionManager.DecryptMessage(content.Text);
                        if (IsContinuation)
                            VisualStateManager.GoToState(((MessageControl)d), "Continuation", false);
                        else
                            VisualStateManager.GoToState(((MessageControl)d), "VisualState", false);
                    }
                    else if (MessageType == MessageTypes.Default)
                    {
                        encrypted.Visibility = Visibility.Collapsed;
                        if (IsContinuation)
                            VisualStateManager.GoToState(((MessageControl)d), "Continuation", false);
                        else
                            VisualStateManager.GoToState(((MessageControl)d), "VisualState", false);
                    }
                }
            }
            if (prop == MessageProperty)
            {
                UpdateMessage();
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

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            var content = Message.Value.Content;
            content = content.Remove(0, 31);
            content = content.Remove(content.Length - 2);
            App.CreateMessage(App.CurrentChannelId, EncryptionManager.GetHandshakeResponse(content));
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

        private async void GatewayOnMessageReactionRemoved(object sender, GatewayEventArgs<MessageReactionUpdate> gatewayEventArgs)
        {
            if (gatewayEventArgs.EventData.MessageId != messageid) return;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (reactionView == null)
                        reactionView = GenerateWrapGrid();
                    ToggleButton toRemove = null;
                    foreach (ToggleButton toggle in reactionView.Children)
                    {
                        var tuple = toggle.Tag as Tuple<string, string, Reactions>;
                        if (tuple.Item3.Emoji.Name == gatewayEventArgs.EventData.Emoji.Name)
                        {
                            var text = ((toggle.Content as StackPanel).Children.Last() as TextBlock).Text;
                            var rt = ((toggle.Content as StackPanel).Children.Last() as TextBlock).RenderTransform = new TranslateTransform();
                            if (text == "1")
                            {
                                toRemove = toggle;
                                break;
                            }
                            Storyboard sb = new Storyboard();
                            DoubleAnimation db = new DoubleAnimation()
                            {
                                To = 24,
                                Duration = TimeSpan.FromMilliseconds(100),
                            };

                            Storyboard.SetTarget(db, rt);
                            Storyboard.SetTargetProperty(db, "Y");
                            sb.Children.Add(db);
                            sb.Begin();
                            sb.Completed += (o, o1) =>
                            {
                                //set the text
                                ((toggle.Content as StackPanel).Children.Last() as TextBlock).Text = (Convert.ToInt32(text) - 1).ToString();

                                Storyboard sb1 = new Storyboard();
                                DoubleAnimation db1 = new DoubleAnimation()
                                {
                                    From = -24,
                                    To = 0,
                                    Duration = TimeSpan.FromMilliseconds(150),
                                    EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut },
                                };

                                Storyboard.SetTarget(db1, rt);
                                Storyboard.SetTargetProperty(db1, "Y");
                                sb1.Children.Add(db1);
                                sb1.Begin();
                            };
                            if (tuple.Item3.Me)
                                toggle.IsChecked = false;
                        }
                    }
                    if (toRemove != null)
                        reactionView.Children.Remove(toRemove);
                });
        }

        private async void GatewayOnMessageReactionAdded(object sender, GatewayEventArgs<MessageReactionUpdate> gatewayEventArgs)
        {
            if (gatewayEventArgs.EventData.MessageId != messageid) return;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
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
                            var text = ((toggle.Content as StackPanel).Children.Last() as TextBlock).Text;
                            //animate from the top
                            var rt = ((toggle.Content as StackPanel).Children.Last() as TextBlock).RenderTransform = new TranslateTransform();
                            Storyboard sb = new Storyboard();
                            DoubleAnimation db = new DoubleAnimation()
                            {
                                To = -24,
                                Duration = TimeSpan.FromMilliseconds(100),
                            };
                           
                            Storyboard.SetTarget(db, rt);
                            Storyboard.SetTargetProperty(db,"Y");
                            sb.Children.Add(db);
                            sb.Begin();
                            sb.Completed += (o, o1) =>
                            {
                                //set the text
                                ((toggle.Content as StackPanel).Children.Last() as TextBlock).Text = (Convert.ToInt32(text) + 1).ToString();

                                Storyboard sb1 = new Storyboard();
                                DoubleAnimation db1 = new DoubleAnimation()
                                {
                                    From = 24,
                                    To = 0,
                                    Duration = TimeSpan.FromMilliseconds(150),
                                    EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut },
                                };

                                Storyboard.SetTarget(db1, rt);
                                Storyboard.SetTargetProperty(db1, "Y");
                                sb1.Children.Add(db1);
                                sb1.Begin();
                            };

                            if (tuple.Item3.Me)
                                toggle.IsChecked = true;
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
        public async void UpdateMessage()
        {
            if (rootGrid.Children.Contains(advert))
                rootGrid.Children.Remove(advert);
            advert = null;
            if (rootGrid.Children.Contains(reactionView))
                rootGrid.Children.Remove(reactionView);
            if (Message.HasValue)
            {
                messageid = Message.Value.Id;
                if (Message.Value.MentionEveryone || (Message.Value.Mentions != null &&  Message.Value.Mentions.Any(x => x.Id == LocalState.CurrentUser.Id)))
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


                if (Message.Value.User.Username != null)
                    username.Content = Message.Value.User.Username;
                else
                    username.Content = "";
                GuildMember member;
                if (Message.Value.User.Id != null) userid = Message.Value.User.Id;
                else userid = "";
                if (App.CurrentGuildId != null && Message.Value.User.Id != null)
                {
                    if (LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(Message.Value.User.Id))
                    {
                        member = LocalState.Guilds[App.CurrentGuildId].members[Message.Value.User.Id];
                    } else
                    {
                        member = new GuildMember() { User = Message.Value.User };
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

                if (LocalState.PresenceDict.ContainsKey(member.User.Id))
                {
                    if (LocalState.PresenceDict[member.User.Id].Status != null && LocalState.PresenceDict[member.User.Id].Status != "invisible")
                        ShadowPresence.Color = (Color)App.Current.Resources[Common.Capitalize(LocalState.PresenceDict[member.User.Id].Status) + "Color"];
                    else if (LocalState.PresenceDict[member.User.Id].Status == "invisible")
                        //ShadowPresence.Color = (Color)App.Current.Resources["OfflineColor"];
                        ShadowPresence.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //ShadowPresence.Color = (Color)App.Current.Resources["OfflineColor"];
                    ShadowPresence.Visibility = Visibility.Collapsed;
                }

                if (member.Roles != null && member.Roles.Any())
                {
                    foreach (Role role in LocalState.Guilds[App.CurrentGuildId].Raw.Roles)
                    {
                        if (role.Id == member.Roles.First<string>())
                        {
                            username.Foreground = IntToColor(role.Color);
                        }
                    }
                }
                else
                {
                    username.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
                }
                if (Message.Value.User.Bot == true)
                    BotIndicator.Visibility = Visibility.Visible;
                else
                    BotIndicator.Visibility = Visibility.Collapsed;
                if (Message.Value.Pinned)
                    MorePin.Text = App.GetString("/Controls/Unpin");
                else
                    MorePin.Text = App.GetString("/Controls/Pin") + " ";

                if (!Storage.Settings.DevMode)
                    MoreCopyId.Visibility = Visibility.Collapsed;

                AvatarBrush.ImageSource = new BitmapImage(Common.AvatarUri(Message.Value.User.Avatar, Message.Value.User.Id));

                if (Message.Value.User.Avatar == null)
                    AvatarBG.Fill = Common.DiscriminatorColor(Message.Value.User.Discriminator);
                else
                    AvatarBG.Fill = Common.GetSolidColorBrush("#00000000");

                timestamp.Text = Common.HumanizeDate(Message.Value.Timestamp, null);
                if (Message.Value.EditedTimestamp.HasValue)
                    timestamp.Text += " (" + App.GetString("/Controls/Edited") + " " +
                                      Common.HumanizeDate(Message.Value.EditedTimestamp.Value,
                                          Message.Value.Timestamp) + ")";

                if (Message.Value.Reactions != null)
                {
                    reactionView = GenerateWrapGrid();
                    foreach (Reactions reaction in Message.Value.Reactions.Where(x => x.Count > 0))
                    {
                        reactionView.Children.Add(GenerateReactionToggle(reaction));
                    }
                    Grid.SetRow(reactionView, 3);
                    Grid.SetColumn(reactionView, 1);
                    rootGrid.Children.Add(reactionView);
                }
                else
                {
                    if (rootGrid.Children.Contains(reactionView))
                        rootGrid.Children.Remove(reactionView);
                    reactionView = null;
                }
                LoadEmbedsAndAttachements();

                content.Users = Message.Value.Mentions;
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
                content.Text = Message.Value.Content;
                string text = Message.Value.Content;
                //string startLink = "";
                string[] Searcheables = new string[] { "https://discord.gg/", "http://discord.gg/", "https://discordapp.com/invite/", "http://discordapp.com/invite/" };
                
                foreach(string link in Searcheables)
                    foreach(int index in AllIndexesOf(text,link))
                    {
                        string text1 = text.Remove(0, index);
                        int interrupt = text1.IndexOf(' ');
                        if (interrupt != -1)
                            text1 = text1.Remove(interrupt);
                        EmbedViewer.Visibility = Visibility.Visible;
                        EmbedViewer.Children.Add(new EmbededInviteControl
                        {
                            InviteCode = text1
                        });
                    }

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
        private ToggleButton GenerateReactionToggle(Reactions reaction)
        {
            ToggleButton reactionToggle = new ToggleButton();
            reactionToggle.IsChecked = reaction.Me;
            reactionToggle.Tag =
                new Tuple<string, string, Reactions>(Message.Value.ChannelId, Message.Value.Id, reaction);
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

            if (!Message.HasValue || (Message.HasValue && Message.Value.Embeds == null)) return;
            if (Message.Value.Embeds.Any() || Message.Value.Attachments.Any())
                EmbedViewer.Visibility = Visibility.Visible;
            
            if (Message.Value.Embeds != null)
            {
                foreach (Embed embed in Message.Value.Embeds)
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
            if (Message.Value.Attachments != null)
            {
                foreach (Attachment attach in Message.Value.Attachments)
                {
                    EmbedViewer.Children.Add(new AttachementControl() { DisplayedAttachement = attach });
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
                if (!LocalState.Guilds[App.CurrentGuildId].channels[Message.Value.ChannelId].permissions.ManageMessages && !LocalState.Guilds[App.CurrentGuildId].channels[Message.Value.ChannelId].permissions.Administrator && Message?.User.Id != LocalState.CurrentUser.Id && LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId != LocalState.CurrentUser.Id)
                {
                    MoreDelete.Visibility = Visibility.Collapsed;
                    MoreEdit.Visibility = Visibility.Collapsed;
                }
                else if (Message?.User.Id != LocalState.CurrentUser.Id)
                {
                    MoreEdit.Visibility = Visibility.Collapsed;
                }
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
                    if (!LocalState.Guilds[App.CurrentGuildId].channels[Message.Value.ChannelId].permissions.ManageMessages && !LocalState.Guilds[App.CurrentGuildId].channels[Message.Value.ChannelId].permissions.Administrator && Message?.User.Id != LocalState.CurrentUser.Id)
                    {
                        MoreDelete.Visibility = Visibility.Collapsed;
                        MoreEdit.Visibility = Visibility.Collapsed;
                    }
                    else if (Message?.User.Id != LocalState.CurrentUser.Id)
                    {
                        MoreEdit.Visibility = Visibility.Collapsed;
                    }
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
                    if (Message?.User.Id == Message.Value.User.Id || !LocalState.Guilds[App.CurrentChannelId].channels[Message.Value.ChannelId].permissions.SendMessages)
                    {
                        MoreReply.Visibility = Visibility.Collapsed;
                    }
                    if (!LocalState.Guilds[App.CurrentGuildId].channels[Message.Value.ChannelId].permissions.ManageMessages && !LocalState.Guilds[App.CurrentGuildId].channels[Message.Value.ChannelId].permissions.Administrator && Message?.User.Id != LocalState.CurrentUser.Id)
                    {
                        MoreDelete.Visibility = Visibility.Collapsed;
                        MoreEdit.Visibility = Visibility.Collapsed;
                    }
                }
                if (Message?.User.Id != LocalState.CurrentUser.Id)
                {
                    MoreEdit.Visibility = Visibility.Collapsed;
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
            editBox = new MessageBox() { Text = EditValue.Trim(),
                                         Background = new SolidColorBrush(Colors.Transparent),
                                         Padding = new Thickness(6,6,12,6),
                                         IsEdit = true };
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
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await RESTCalls.EditMessageAsync(Message.Value.ChannelId, Message.Value.Id, editBox.Text));
            editBox.Send -= EditBox_Send;
            editBox.Cancel -= EditBox_Cancel;
            editBox.TextChanged -= EditBox_TextChanged;
            editBox.LostFocus -= EditBox_Cancel;
            rootGrid.Children.Remove(editBox);
            content.Visibility = Visibility.Visible;
        }

        private async void content_LinkClicked(object sender, MarkdownTextBlock.LinkClickedEventArgs e)
        {
            //LinkClicked(sender, e);
            await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
        }

        private async void MorePin_Click(object sender, RoutedEventArgs e)
        {
            if (Message.Value.Pinned)
            {
                await RESTCalls.UnpinMessage(Message.Value.ChannelId, Message.Value.Id);
            } else
            {
                await RESTCalls.PinMesage(Message.Value.ChannelId, Message.Value.Id);
            }
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            await RESTCalls.DeleteMessage(Message.Value.ChannelId, Message.Value.Id);
        }

        private void MoreCopyId_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(Message.Value.Id);
            Clipboard.SetContent(dataPackage);
        }

        private void Username_OnClick(object sender, RoutedEventArgs e)
        {
            App.ShowMemberFlyout(username, Message.Value.User);
        }

        private void username_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (!App.CurrentGuildIsDM)
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, Message.Value.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        private void username_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                if (!App.CurrentGuildIsDM)
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, Message.Value.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        Flyout PickReaction;
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
            PickReaction.Hide();
            string emojiStr = e.surrogates;
            if(e.GetType() == typeof(EmojiControl.GuildSide))
            {
                var emoji = (EmojiControl.GuildSide)e;
                emojiStr = emoji.names[0] + ":" + emoji.id;
            }
            await RESTCalls.CreateReactionAsync(Message.Value.ChannelId, messageid, emojiStr);
        }

        private void MoreReply_Click(object sender, RoutedEventArgs e)
        {
            App.MentionUser(Message.Value.User.Username, Message.Value.User.Discriminator);
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
    }
}
