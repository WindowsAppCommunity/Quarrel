using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
using Discord_UWP.CacheModels;
using Discord_UWP.Controls;
using Discord_UWP.SharedModels;
#region CacheModels Overrule
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;
using Message = Discord_UWP.CacheModels.Message;
using User = Discord_UWP.CacheModels.User;
using Guild = Discord_UWP.CacheModels.Guild;
#endregion
/* The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236 */

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


        //Calls OnPropertyChanged
        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as MessageControl;

            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }
        AdControl advert;
        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == IsContinuationProperty)
            {
                if (IsContinuation)
                    VisualStateManager.GoToState(((MessageControl)d), "Continuation", false);
                else
                    VisualStateManager.GoToState(((MessageControl)d), "VisualState", false);
            }


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
           // RegisterPropertyChangedCallback(MessageProperty, OnPropertyChanged);
           // RegisterPropertyChangedCallback(HeaderProperty, OnPropertyChanged);
           // RegisterPropertyChangedCallback(IsContinuationProperty, OnPropertyChanged);
           // RegisterPropertyChangedCallback(IsAdvertProperty, OnPropertyChanged);
        }

        private GridView reactionView;
        public void UpdateMessage()
        {
            if (Message.HasValue)
            {
                if (Message.Value.User.Username != null)
                    username.Text = Message.Value.User.Username;
                else
                    username.Text = "";
                GuildMember member;
                if (App.CurrentId != null && Storage.Cache.Guilds[App.CurrentId]
                        .Members.ContainsKey(Message.Value.User.Id))
                {
                    member = Storage.Cache.Guilds[App.CurrentId].Members[Message.Value.User.Id].Raw;
                }
                else
                {
                    member = new GuildMember();
                }
                if (member.Nick != null)
                {
                    username.Text = member.Nick;
                }

                if (member.Roles != null && member.Roles.Count() > 0)
                {
                    foreach (Role role in Storage.Cache.Guilds[App.CurrentId].RawGuild.Roles)
                    {
                        if (role.Id == member.Roles.First<string>())
                        {
                            username.Foreground = IntToColor(role.Color);
                        }
                    }
                }

                if (Message.Value.User.Bot == true)
                    BotIndicator.Visibility = Visibility.Visible;
                if (Message.Value.Pinned)
                    MorePin.Text = "Unpin";

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
                        Margin = new Thickness(0),
                        Padding = new Thickness(0)
                    };
                    foreach (Reactions reaction in Message.Value.Reactions.Where(x => x.Count > 0))
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

                        reactionToggle.Content = reaction.Emoji.Name + " " + reaction.Count.ToString();
                        ;
                        reactionToggle.Style = (Style) App.Current.Resources["EmojiButton"];
                        reactionToggle.MinHeight = 0;

                        GridViewItem gridViewItem = new GridViewItem() {Content = reactionToggle};

                        gridViewItem.MinHeight = 0;
                        reactionView.Margin = new Thickness(6, 0, 0, 0);
                        reactionView.Items.Add(gridViewItem);
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
                content.Text = Message.Value.Content;
            }
            else
            {
                username.Text = "";
                avatar.Fill = null;
                timestamp.Text = "";
                content.Text = "";
                if (rootGrid.Children.Contains(reactionView))
                    rootGrid.Children.Remove(reactionView);
                reactionView = null;
            }
        }

        private void LoadEmbeds()
        {
            EmbedViewer.Visibility = Visibility.Collapsed;
            EmbedViewer.Children.Clear();
            if (!Message.HasValue) return;

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
            if (Message.Value.Attachments.Any())
            {
                var attachement = Message.Value.Attachments.First();
                bool IsImage = false;
                if (EnableImages)
                {
                    foreach (string ext in ImageFiletypes)
                        if (attachement.Filename.ToLower().EndsWith(ext))
                        {
                            IsImage = true;
                            if (attachement.Filename.EndsWith(".svg"))
                                AttachedImageViewer.Source = new SvgImageSource(new Uri(attachement.Url));
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
            if (perms == null)
            {
                perms = new Permissions();
                if (App.CurrentId != null)
                {
                    foreach (Role role in Storage.Cache.Guilds[App.CurrentId].RawGuild.Roles)
                    {
                        if (!Storage.Cache.Guilds[App.CurrentId].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id))
                        {
                            Storage.Cache.Guilds[App.CurrentId].Members.Add(Storage.Cache.CurrentUser.Raw.Id, new Member(Session.GetGuildMember(App.CurrentId, Storage.Cache.CurrentUser.Raw.Id)));
                        }

                        if (Storage.Cache.Guilds[App.CurrentId].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Count() != 0 && Storage.Cache.Guilds[App.CurrentId].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.First().ToString() == role.Id)
                        {
                            perms.GetPermissions(role, Storage.Cache.Guilds[App.CurrentId].RawGuild.Roles);
                        }
                        else
                        {
                            perms.GetPermissions(0);
                        }
                    }
                }
            }

            if (!perms.EffectivePerms.ManageMessages && !perms.EffectivePerms.Administrator && Message?.User.Id != Storage.Cache.CurrentUser.Raw.Id)
            {
                MoreDelete.Visibility = Visibility.Collapsed;
            }
            else if (Message?.User.Id != Storage.Cache.CurrentUser.Raw.Id)
            {
                MoreEdit.Visibility = Visibility.Collapsed;
            }
            FlyoutBase.ShowAttachedFlyout(sender as Button);
        }

        private void UserControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(moreButton);
        }

        private void UserControl_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(moreButton);
        }

        private void ToggleReaction(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton)?.IsChecked == false) //Inverted since it changed
            {
                Session.DeleteReaction(((sender as ToggleButton).Tag as Tuple<string, string, Reactions>)?.Item1, ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>)?.Item2, ((Tuple<string, string, Reactions>) (sender as ToggleButton).Tag).Item3.Emoji);
                if (((Tuple<string, string, Reactions>) ((ToggleButton) sender).Tag).Item3.Me)
                {
                    ((ToggleButton) sender).Content = (((ToggleButton) sender).Tag as Tuple<string, string, Reactions>)?.Item3.Emoji.Name + " " + (((Tuple<string, string, Reactions>) ((ToggleButton) sender).Tag).Item3.Count - 1).ToString();
                }
                else
                {
                    ((ToggleButton) sender).Content = (((ToggleButton) sender).Tag as Tuple<string, string, Reactions>)?.Item3.Emoji.Name + " " + (((Tuple<string, string, Reactions>) ((ToggleButton) sender).Tag).Item3.Count).ToString();
                }
            }
            else
            {
                Session.CreateReaction((((ToggleButton) sender).Tag as Tuple<string, string, Reactions>)?.Item1, ((Tuple<string, string, Reactions>) ((ToggleButton) sender).Tag).Item2, ((Tuple<string, string, Reactions>) ((ToggleButton) sender).Tag).Item3.Emoji);

                if (((Tuple<string, string, Reactions>) ((ToggleButton) sender).Tag).Item3.Me)
                {
                    ((ToggleButton) sender).Content = (((ToggleButton) sender).Tag as Tuple<string, string, Reactions>)?.Item3.Emoji.Name + " " + (((Tuple<string, string, Reactions>) ((ToggleButton) sender).Tag).Item3.Count).ToString();
                }
                else
                {
                    ((ToggleButton) sender).Content = ((Tuple<string, string, Reactions>) ((ToggleButton) sender).Tag).Item3.Emoji.Name + " " + (((Tuple<string, string, Reactions>) ((ToggleButton) sender).Tag).Item3.Count + 1).ToString();
                }
            }
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {           
            string val = "";
            MessageBox.Document.GetText(TextGetOptions.None, out val);
            if (val.Trim() == "")
                MessageBox.Document.SetText(TextSetOptions.None, content.Text);
            EditBox.Visibility = Visibility.Visible;
        }

        private void CreateMessage(object sender, RoutedEventArgs e)
        {
            string editedText = "";
            MessageBox.Document.GetText(TextGetOptions.None, out editedText);
            Session.EditMessage(Message.Value.ChannelId, Message.Value.Id, editedText);
        }

        private void content_LinkClicked(object sender, MarkdownTextBlock.LinkClickedEventArgs e)
        {
            LinkClicked(sender, e);
        }

        private void MessageBox_TextChanged(object sender, RoutedEventArgs e)
        {
            string text = "";
            MessageBox.Document.GetText(TextGetOptions.None, out text);
            if (text != "")
                SendBox.IsEnabled = true;
            else
                SendBox.IsEnabled = false;
        }
        Permissions perms;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditBox.Visibility = Visibility.Collapsed;
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
    }
}
