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
using Discord_UWP.CacheModels;
using Discord_UWP.Controls;
using Discord_UWP.Gateway;
using Discord_UWP.Gateway.DownstreamEvents;
using Discord_UWP.SharedModels;
#region CacheModels Overrule
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;
using Message = Discord_UWP.CacheModels.Message;
using User = Discord_UWP.CacheModels.User;
using Guild = Discord_UWP.CacheModels.Guild;
#endregion


namespace Discord_UWP
{
    public sealed partial class MessageControl : UserControl
    {
        /*<summary>
        Fired when a link element in the markdown was tapped.
        </summary>*/

        public event EventHandler<MarkdownTextBlock.LinkClickedEventArgs> LinkClicked;

        //Is the more button visible?
        public Visibility MoreButtonVisibility
        {
            get { return moreButton.Visibility; }
            set { moreButton.Visibility = value; }
        }

        //Is the message an advert?
        public bool IsAdvert
        {
            get { return (bool)GetValue(IsAdvertProperty); }
            set { SetValue(IsAdvertProperty, value); }
        }
        public static readonly DependencyProperty IsAdvertProperty = DependencyProperty.Register(
            nameof(IsAdvert),
            typeof(bool),
            typeof(MessageControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

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

            if (prop == IsAdvertProperty)
            {
                if (IsAdvert)
                {
                    VisualStateManager.GoToState(this, "Advert", false);
                    advert = new AdControl();
                    advert.HorizontalAlignment = HorizontalAlignment.Center;
                    advert.Width = 300;
                    advert.Height = 50;
                    advert.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                    advert.AdUnitId = "336795";
                    advert.Margin = new Thickness(6);
                    advert.Background = new SolidColorBrush(Colors.Red);
                    Grid.SetColumnSpan(advert, 10);
                    Grid.SetRowSpan(advert, 10);
                    rootGrid.Children.Add(advert);
                    return;
                }
                else
                {
                    if (rootGrid.Children.Contains(advert))
                        rootGrid.Children.Remove(advert);
                    advert = null;
                    VisualStateManager.GoToState(this, "VisualState", false);
                }
            }
            if (prop == MessageProperty)
            {
                UpdateMessage();
            }
        }

        public MessageControl()
        {
            this.InitializeComponent();
            Session.Gateway.MessageReactionAdded += GatewayOnMessageReactionAdded;
            Session.Gateway.MessageReactionRemoved += GatewayOnMessageReactionRemoved;
        }

        private async void GatewayOnMessageReactionRemoved(object sender, GatewayEventArgs<MessageReactionUpdate> gatewayEventArgs)
        {
            if (gatewayEventArgs.EventData.MessageId != messageid) return;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                        if (reactionView == null)
                            reactionView = new GridView
                            {
                                SelectionMode = ListViewSelectionMode.None,
                                Margin = new Thickness(0),
                                Padding = new Thickness(0)
                            };
                    ToggleButton toRemove = null;
                    foreach (ToggleButton toggle in reactionView.Items)
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
                        reactionView.Items.Remove(toRemove);
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
                        reactionView = new GridView
                        {
                            SelectionMode = ListViewSelectionMode.None,
                            Margin = new Thickness(6, 0, 0, 0),
                            Padding = new Thickness(0)
                        };
                        rootGrid.Children.Add(reactionView);
                    }


                    bool success = false;
                    foreach (ToggleButton toggle in reactionView.Items)
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
                            Me = App.CurrentUserId.Equals(data.UserId),
                        });
                        reactionView.Items.Add(toggle);
                    }
                });
        }

        private void Sb_Completed(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private GridView reactionView;
        private Attachment attachement;
        private string userid = "";
        public string messageid = "";
        public void UpdateMessage()
        {
            if (Message.HasValue)
            {
                messageid = Message.Value.Id;
                if (Message.Value.MentionEveryone || Message.Value.Mentions.Any(x => x.Id == App.CurrentUserId))
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
                if (App.CurrentGuildId != null && Storage.Cache.Guilds[App.CurrentGuildId].Members.ContainsKey(Message.Value.User.Id))
                {
                    member = Storage.Cache.Guilds[App.CurrentGuildId].Members[Message.Value.User.Id].Raw;
                }
                else
                {
                    member = new GuildMember();
                }
                if (member.Nick != null)
                {
                    username.Content = member.Nick;
                }

                if (member.Roles != null && member.Roles.Any())
                {
                    foreach (Role role in App.CurrentGuild.RawGuild.Roles)
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
                    MorePin.Text = "Unpin";
                else
                    MorePin.Text = "Pin";

                if (!Storage.Settings.DevMode)
                    MoreCopyId.Visibility = Visibility.Collapsed;

                if (!string.IsNullOrEmpty(Message.Value.User.Avatar))
                {
                    avatar.Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(
                            new Uri("https://cdn.discordapp.com/avatars/" + Message.Value.User.Id + "/" +
                                    Message.Value.User.Avatar + ".jpg"))
                    };
                }
                else
                {
                    avatar.Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"))
                    };
                }

                timestamp.Text = Common.HumanizeDate(Message.Value.Timestamp, null);
                if (Message.Value.EditedTimestamp.HasValue)
                    timestamp.Text += " (Edited " +
                                      Common.HumanizeDate(Message.Value.EditedTimestamp.Value,
                                          Message.Value.Timestamp) + ")";

                if (Message.Value.Reactions != null)
                {
                    reactionView = new GridView
                    {
                        SelectionMode = ListViewSelectionMode.None,
                        Margin = new Thickness(6, 0, 0, 0),
                        Padding = new Thickness(0)

                    };
                    foreach (Reactions reaction in Message.Value.Reactions.Where(x => x.Count > 0))
                    {

                        reactionView.Items.Add(GenerateReactionToggle(reaction));
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
                LoadAttachements(true);
                LoadEmbeds();
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
                string startLink = "";
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
            }
            else
            {
                messageid = "";
                content.Visibility = Visibility.Visible;
                Grid.SetRow(moreButton,2);
                username.Content = "";
                avatar.Fill = null;
                timestamp.Text = "";
                content.Text = "";
                if (rootGrid.Children.Contains(reactionView))
                    rootGrid.Children.Remove(reactionView);
                reactionView = null;

                /* The resetting of the embed and attachement related stuff is handled by these functions: */
                LoadEmbeds();
                LoadAttachements(false);
            }
        }
        public static IEnumerable<int> AllIndexesOf(string str, string searchstring)
        {
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
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
            Debug.WriteLine(reaction.Emoji.Name);
            if (App.CurrentGuild.RawGuild.Emojis != null)
                foreach (Emoji emoji in App.CurrentGuild.RawGuild.Emojis)
                {
                    if (emoji.Name == reaction.Emoji.Name)
                    {
                        serversideEmoji = emoji.Id;
                    }
                }
            if (serversideEmoji != null)
            {
                stack.Children.Add(new Image()
                {
                    Width = 20,
                    Height = 20,
                    Source = new BitmapImage(new Uri("https://cdn.discordapp.com/emojis/" + serversideEmoji + ".png"))
                });
            }
            else
            {
                stack.Children.Add(new TextBlock()
                {
                    FontSize = 20,
                    Text = reaction.Emoji.Name,
                    FontFamily = new FontFamily("ms-appx:/Assets/emojifont.ttf#Twitter Color Emoji")
                });
            }
            stack.Children.Add(new TextBlock() { Text = reaction.Count.ToString(), Margin = new Thickness(4, 0, 0, 0) });
            stack.Clip = new RectangleGeometry(){Rect=new Rect(0,-4,96,28)};
            reactionToggle.Content = stack;
            reactionToggle.Style = (Style)App.Current.Resources["EmojiButton"];
            reactionToggle.MinHeight = 0;
            return reactionToggle;
        }
        private void LoadEmbeds()
        {
            EmbedViewer.Visibility = Visibility.Collapsed;
            EmbedViewer.Children.Clear();

            if (!Message.HasValue || (Message.HasValue && Message.Value.Embeds == null)) return;
            if (Message.Value.Embeds.Any())
                EmbedViewer.Visibility = Visibility.Visible;
            foreach (Embed embed in Message.Value.Embeds)
            {
                EmbedViewer.Children.Add(new EmbedControl(){Content = embed});
            }
        }

        readonly string[] ImageFiletypes = { ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".png", ".bmp", ".gif", ".ico" };
        private void LoadAttachements(bool EnableImages)
        {
            AttachedImageViewer.Source = null;
            AttachedImageViewbox.Visibility = Visibility.Collapsed;
            AttachedImageViewer.ImageOpened -= AttachedImageViewer_ImageLoaded;
            AttachedImageViewer.ImageFailed -= AttachementImageViewer_ImageFailed;
            LoadingImage.IsActive = false;
            LoadingImage.Visibility = Visibility.Collapsed;
            AttachedFileViewer.Visibility = Visibility.Collapsed;
            if (!Message.HasValue || (Message.HasValue && Message.Value.Attachments == null)) return;
            if (Message.Value.Attachments.Any())
            {
                attachement = Message.Value.Attachments.First();
                bool IsImage = false;
                if (EnableImages)
                {
                    foreach (string ext in ImageFiletypes)
                        if (attachement.Filename.ToLower().EndsWith(ext))
                        {
                            IsImage = true;
                            if (attachement.Filename.EndsWith(".svg"))
                            {
                                AttachedImageViewer.Source = new SvgImageSource(new Uri(attachement.Url));
                            }
                               
                            else
                            {
                                AttachedImageViewer.Source = new BitmapImage(new Uri(attachement.Url));
                            }
                            break;
                        }
                }
                if (IsImage)
                {
                    AttachedImageViewbox.Visibility = Visibility.Visible;
                    LoadingImage.Visibility = Visibility.Visible;
                    LoadingImage.IsActive = true;
                    AttachedImageViewer.ImageOpened += AttachedImageViewer_ImageLoaded;
                    AttachedImageViewer.ImageFailed += AttachementImageViewer_ImageFailed;
                }
                else
                {
                    FileName.NavigateUri = new Uri(attachement.Url);
                    FileName.Content = attachement.Filename;
                    FileSize.Text = HumanizeFileSize(attachement.Size);
                    AttachedFileViewer.Visibility = Visibility.Visible;
                }
            }
        }
        private void AttachedImageViewer_ImageLoaded(object sender, RoutedEventArgs e)
        {
            AttachedImageViewer.ImageOpened -= AttachedImageViewer_ImageLoaded;
            AttachedImageViewer.ImageFailed -= AttachementImageViewer_ImageFailed;
            LoadingImage.IsActive = false;
            LoadingImage.Visibility=Visibility.Collapsed;
        }

        private void AttachementImageViewer_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            AttachedImageViewer.ImageOpened -= AttachedImageViewer_ImageLoaded;
            AttachedImageViewer.ImageFailed -= AttachementImageViewer_ImageFailed;
            LoadingImage.IsActive = false;
            LoadingImage.Visibility = Visibility.Collapsed;
            //Reload attachements but with images disabled
            LoadAttachements(false);
        }

        private void moreButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentGuildId != null)
            {
                if (!Storage.Cache.Guilds[App.CurrentGuildId].Channels[Message.Value.ChannelId].chnPerms.Perms.ManageMessages && !Storage.Cache.Guilds[App.CurrentGuildId].Channels[Message.Value.ChannelId].chnPerms.Perms.Administrator && Message?.User.Id != Storage.Cache.CurrentUser.Raw.Id && Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.OwnerId != Storage.Cache.CurrentUser.Raw.Id)
                {
                    MoreDelete.Visibility = Visibility.Collapsed;
                    MoreEdit.Visibility = Visibility.Collapsed;
                }
                else if (Message?.User.Id != Storage.Cache.CurrentUser.Raw.Id)
                {
                    MoreEdit.Visibility = Visibility.Collapsed;
                }
            }
            FlyoutBase.ShowAttachedFlyout(sender as Button);
        }

        private void UserControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (App.CurrentGuildId != null)
            {
                if (!Storage.Cache.Guilds[App.CurrentGuildId].Channels[Message.Value.ChannelId].chnPerms.Perms.ManageMessages && !Storage.Cache.Guilds[App.CurrentGuildId].Channels[Message.Value.ChannelId].chnPerms.Perms.Administrator && Message?.User.Id != Storage.Cache.CurrentUser.Raw.Id)
                {
                    MoreDelete.Visibility = Visibility.Collapsed;
                    MoreEdit.Visibility = Visibility.Collapsed;
                }
                else if (Message?.User.Id != Storage.Cache.CurrentUser.Raw.Id)
                {
                    MoreEdit.Visibility = Visibility.Collapsed;
                }
            }
            FlyoutBase.ShowAttachedFlyout(moreButton);
        }

        private void UserControl_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (App.CurrentGuildId != null)
            {
                if (!Storage.Cache.Guilds[App.CurrentGuildId].Channels[Message.Value.ChannelId].chnPerms.Perms.ManageMessages && !Storage.Cache.Guilds[App.CurrentGuildId].Channels[Message.Value.ChannelId].chnPerms.Perms.Administrator && Message?.User.Id != Storage.Cache.CurrentUser.Raw.Id)
                {
                    MoreDelete.Visibility = Visibility.Collapsed;
                    MoreEdit.Visibility = Visibility.Collapsed;
                }
                else if (Message?.User.Id != Storage.Cache.CurrentUser.Raw.Id)
                {
                    MoreEdit.Visibility = Visibility.Collapsed;
                }
            }
            FlyoutBase.ShowAttachedFlyout(moreButton);
        }

        private async void ToggleReaction(object sender, RoutedEventArgs e)
        {
            var counter = ((StackPanel) ((ToggleButton) sender).Content).Children.Last() as TextBlock;
            if ((sender as ToggleButton)?.IsChecked == false) //Inverted since it changed
            {
                await Session.DeleteReactionAsync(((sender as ToggleButton).Tag as Tuple<string, string, Reactions>)?.Item1, ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>)?.Item2, ((Tuple<string, string, Reactions>)(sender as ToggleButton).Tag).Item3.Emoji);
               // if (((Tuple<string, string, Reactions>)((ToggleButton)sender).Tag).Item3.Me)
               // {
               //     counter.Text = (((Tuple<string, string, Reactions>)((ToggleButton)sender).Tag).Item3.Count - 1).ToString();
               // }
               // else
               // {
               //     counter.Text = (((Tuple<string, string, Reactions>)((ToggleButton)sender).Tag).Item3.Count).ToString();
               // }
            }
            else
            {
                await Session.CreateReactionAsync((((ToggleButton)sender).Tag as Tuple<string, string, Reactions>)?.Item1, ((Tuple<string, string, Reactions>)((ToggleButton)sender).Tag).Item2, ((Tuple<string, string, Reactions>)((ToggleButton)sender).Tag).Item3.Emoji);

              //  if (((Tuple<string, string, Reactions>)((ToggleButton)sender).Tag).Item3.Me)
              //  {
              //      counter.Text = (((Tuple<string, string, Reactions>)((ToggleButton)sender).Tag).Item3.Count).ToString();
              //  }
              //  else
              //  {
              //      counter.Text = (((Tuple<string, string, Reactions>)((ToggleButton)sender).Tag).Item3.Count + 1).ToString();
              //  }
            }
        }

        string EditValue = "";
        MessageBox editBox;
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (EditValue.Trim() == "") EditValue = content.Text;
            editBox = new MessageBox() { Text = EditValue.Trim(),
                                         Background =(SolidColorBrush)App.Current.Resources["DeepBG"],
                                         Padding = new Thickness(6,6,12,6),
                                         IsEdit = true };
            editBox.Send += EditBox_Send;
            editBox.Cancel += EditBox_Cancel;
            editBox.TextChanged += EditBox_TextChanged;
            editBox.LostFocus += EditBox_Cancel;
            Grid.SetRow(editBox, 2);
            Grid.SetColumn(editBox, 1);
            rootGrid.Children.Add(editBox);
        }

        private void EditBox_Cancel(object sender, RoutedEventArgs e)
        {
            editBox.Send += EditBox_Send;
            editBox.Cancel += EditBox_Cancel;
            editBox.TextChanged += EditBox_TextChanged;
            editBox.LostFocus += EditBox_Cancel;
            rootGrid.Children.Remove(editBox);
        }

        private void EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EditValue = editBox.Text;
        }

        private async void EditBox_Send(object sender, RoutedEventArgs e)
        {
            editBox.Send -= EditBox_Send;
            editBox.TextChanged -= EditBox_TextChanged;
            string chnId = Message.Value.ChannelId;
            string msgId = Message.Value.Id;
            string newMeg = editBox.Text;
            editBox.IsEnabled = false;
            await Task.Run(() => Session.EditMessageAsync(chnId, msgId, newMeg));
            rootGrid.Children.Remove(editBox);
        }


        private async void content_LinkClicked(object sender, MarkdownTextBlock.LinkClickedEventArgs e)
        {
            //LinkClicked(sender, e);
            await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
        }


        private void MorePin_Click(object sender, RoutedEventArgs e)
        {
            if (Message.Value.Pinned)
            {
                Session.UnpinMessage(Message.Value.ChannelId, Message.Value.Id);
            } else
            {
                Session.PinMesage(Message.Value.ChannelId, Message.Value.Id);
            }
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            Session.DeleteMessage(Message.Value.ChannelId, Message.Value.Id);
        }

        private void MoreCopyId_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(Message.Value.Id);
            Clipboard.SetContent(dataPackage);
        }

        private void AttachedImageViewer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.OpenAttachement(attachement);
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
                    App.ShowMenuFlyout(this, App.Type.GuildMember, Message.Value.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        private void username_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                if (!App.CurrentGuildIsDM)
                    App.ShowMenuFlyout(this, App.Type.GuildMember, Message.Value.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }
    }
}
