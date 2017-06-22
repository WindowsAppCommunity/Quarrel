using Discord_UWP.API;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Text;
using System.Diagnostics;
using System.Globalization;
using static Discord_UWP.Common;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.ComponentModel;

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        public static SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
        }
        public static SolidColorBrush GetSolidColorBrush(int alpha, int red, int green, int blue)
        {
            byte a = (byte)alpha;
            byte r = (byte)red;
            byte g = (byte)green;
            byte b = (byte)blue;
            return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
        }
        public static SolidColorBrush GetSolidColorBrush(Windows.UI.Color color)
        {
            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
        }

        public class MessageContainer : INotifyPropertyChanged
        {
            private SharedModels.Message? _message;
            public SharedModels.Message? Message {
                get => _message;
                set { if (Equals(_message, value)) return; _message = value; OnPropertyChanged("Message"); } }

            private bool _iscontinuation;
            public bool IsContinuation{
                get => _iscontinuation;
                set { if (_iscontinuation == value) return; _iscontinuation = value; OnPropertyChanged("IsContinuation"); } }

            private bool _isadvert;
            public bool IsAdvert{
                get => _isadvert;
                set { if (_isadvert == value) return; _isadvert = value; OnPropertyChanged("IsAdvert"); } }


            private string _header;
            public string Header {
                get => _header;
                set { if (_header == value) return; _header = value; OnPropertyChanged("Header"); } }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MessageContainer NewMessageContainer(SharedModels.Message? message, bool? isContinuation, bool isAdvert, string header, bool HideMoreButton = false)
        {
            
            if (!isContinuation.HasValue)
            {
                if (Messages.Items.Last() != null && Messages.Items.Last().GetType() == typeof(MessageContainer))
                {
                    MessageContainer previousMessage = Messages.Items.Last() as MessageContainer;
                    //If the previous message is from the same user and there is less than a two minute difference between the two messages, render it without the message headers
                    if (message != null && previousMessage?.Message != null)
                    {
                        var timedif = (message).Value.Timestamp.Subtract(previousMessage.Message.Value.Timestamp)
                            .TotalSeconds;
                        if (previousMessage.Message.Value.User.Id == message.Value.User.Id && timedif < 20)
                            isContinuation = true;
                        else
                            isContinuation = false;
                    }
                    else isContinuation = false;
                }
                else
                    isContinuation = false;
            }
            return new MessageContainer()
            {
                Header = header,
                IsAdvert = isAdvert,
                IsContinuation = isContinuation.Value,
                
                Message = message
            };
        }

        private UIElement GuildRender(CacheModels.Guild guild)
        {
            ListViewItem listviewitem = new ListViewItem();
            //listviewitem.Margin = new Thickness(0, 5, 0 ,5);
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            if (guild.RawGuild.Icon != null && Session.Online)
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(8, 4, 8, 4);
                Rectangle rect = new Rectangle();
                rect.Height = 48;
                rect.Width = 48;
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                ImageBrush guildimage = new ImageBrush();
                //guildimage.Stretch = Stretch.Uniform;
                guildimage.ImageSource = new BitmapImage(new Uri("https://discordapp.com/api/guilds/" + guild.RawGuild.Id + "/icons/" + guild.RawGuild.Icon + ".jpg"));
                rect.Fill = guildimage;
                //stack.Children.Add(rect);
                //Image icon = new Image();
                //icon.Height = 50;
                //icon.Margin = new Thickness(-10, 0, 0, 0);
                //icon.Source = new BitmapImage(new Uri("https://discordapp.com/api/guilds/" + guild.Id + "/icons/" + guild.Icon + ".jpg"));
                grid.Children.Add(rect);
                stack.Children.Add(grid);
            }
            else
            {
                Grid grid = new Grid();
                //grid.Background = GetSolidColorBrush("#FF738AD6");
                grid.Margin = new Thickness(8, 4, 8, 4);
                grid.Height = 48;
                grid.Width = 48;
                Rectangle rect = new Rectangle();
                rect.Fill = (SolidColorBrush)App.Current.Resources["Blurple"];
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                grid.Children.Add(rect);
                TextBlock icon = new TextBlock();
                icon.Text = guild.RawGuild.Name[0].ToString();
                icon.HorizontalAlignment = HorizontalAlignment.Center;
                icon.VerticalAlignment = VerticalAlignment.Center;
                grid.Children.Add(icon);
                stack.Children.Add(grid);
            }
            TextBlock txtblock = new TextBlock();
            txtblock.Text = guild.RawGuild.Name;
            txtblock.VerticalAlignment = VerticalAlignment.Center;
            stack.Children.Add(txtblock);

            Button serverSettings = new Button()
            {
                Content = new TextBlock() { Text = "", FontFamily = new FontFamily("Segoe MDL2 Assets") },
                Background = GetSolidColorBrush("#00000000"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Tag = guild.RawGuild.Id,
                IsEnabled = false
            };
            serverSettings.Click += OpenGuildSettings;
            stack.Children.Add(serverSettings);
            listviewitem.Height = 56;
            listviewitem.Content = stack;
            listviewitem.Tag = guild.RawGuild.Id;
            ToolTipService.SetToolTip(listviewitem, guild.RawGuild.Name);
            return listviewitem;
        }

        private UIElement GuildMemberRender(SharedModels.GuildMember member)
        {
            ListViewItem listviewitem = new ListViewItem();
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            Grid image = new Grid();
            image.Height = 32;
            image.Width = 32;
            Rectangle avatar = new Rectangle();
            avatar.RadiusX = 100;
            avatar.RadiusY = 100;
            avatar.Height = 32;
            avatar.Width = 32;
            avatar.HorizontalAlignment = HorizontalAlignment.Left;
            avatar.VerticalAlignment = VerticalAlignment.Top;
            if (member.User.Avatar != "" && member.User.Avatar != null)
            {
                avatar.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + member.User.Id + "/" + member.User.Avatar + ".jpg")) };
            } else
            {
                avatar.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png")) };
            }
            image.Children.Add(avatar);
            if (Session.PrecenseDict.ContainsKey(member.User.Id))
            {
                Rectangle rect = new Rectangle();
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                rect.Height = 12;
                rect.Width = 12;
                rect.HorizontalAlignment = HorizontalAlignment.Right;
                rect.VerticalAlignment = VerticalAlignment.Bottom;

                switch (Session.PrecenseDict[member.User.Id].Status)
                {
                    case "online":
                        rect.Fill = GetSolidColorBrush("#ff43b581");
                        break;
                    case "idle":
                        rect.Fill = GetSolidColorBrush("#fffaa61a");
                        break;
                    case "offline":
                        rect.Fill = GetSolidColorBrush("#FFAAAAAA");
                        break;
                }

                image.Children.Add(rect);
            }
            else
            {
                Rectangle rect = new Rectangle();
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                rect.Height = 10;
                rect.Width = 10;
                rect.HorizontalAlignment = HorizontalAlignment.Right;
                rect.VerticalAlignment = VerticalAlignment.Bottom;
                rect.Fill = GetSolidColorBrush("#FFAAAAAA");
                image.Children.Add(rect);
            }

            stack.Children.Add(image);

            StackPanel vertstack = new StackPanel();
            TextBlock txtblock = new TextBlock();
            txtblock.Margin = new Thickness(12);
            txtblock.TextWrapping = TextWrapping.Wrap;
            txtblock.FontSize = 15;
            txtblock.VerticalAlignment = VerticalAlignment.Center;
            txtblock.FontWeight = FontWeights.SemiBold;
            if (member.Nick != null)
            {
                txtblock.Text = member.Nick;
            }
            else
            {
                txtblock.Text = member.User.Username;
            }

            if (member.User.Bot)
            {
                txtblock.Text += " (BOT)";
            }

            if (member.Roles != null && member.Roles.Count() > 0 && Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles != null)
            {
                foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                {
                    if (role.Id == member.Roles.First<string>())
                    {
                        txtblock.Foreground = IntToColor(role.Color);
                    }
                }
            }

            vertstack.Children.Add(txtblock);

            if (Session.PrecenseDict.ContainsKey(member.User.Id))
            {
                if (Session.PrecenseDict[member.User.Id].Game != null)
                {
                    TextBlock game = new TextBlock();
                    game.Text = "Playing " + Session.PrecenseDict[member.User.Id].Game.Value.Name;
                    vertstack.Children.Add(game);
                }
            }

            stack.Children.Add(vertstack);
            stack.Padding = new Thickness(0);
            listviewitem.Content = stack;

            #region Flyout
            Flyout flyout = new Flyout();
            StackPanel flyoutcontent = new StackPanel();
            flyoutcontent.Margin = new Thickness(-10);
            Button profile = new Button()
            {
                Content = "Profile",
                Tag = new Tuple<string, string>((ServerList.SelectedItem as ListViewItem).Tag.ToString(), member.User.Id),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            profile.Click += OpenProfile;
            flyoutcontent.Children.Add(profile);
            Button mention = new Button()
            {
                Content = "Mention",
                Tag = "@" + member.User.Username + "#" + member.User.Discriminator,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            mention.Click += AddMention;
            flyoutcontent.Children.Add(mention);
            //flyoutcontent.Children.Add(Mention);
            //Button Message = new Button()
            //{
            //    Content = "Message",
            //    Tag = member.User.Id,
            //    HorizontalAlignment = HorizontalAlignment.Stretch
            //};
            //Message.Click += DMessage;
            //flyoutcontent.Children.Add(Message);
            //ToggleButton Mute = new ToggleButton()
            //{
            //    Content = "Mute",
            //    HorizontalAlignment = HorizontalAlignment.Stretch
            //};
            //if (member.Mute)
            //{
            //    Mute.IsChecked = true;
            //}
            //flyoutcontent.Children.Add(Mute);
            //Button Friend = new Button()
            //{
            //    Content = "Add Friend",
            //    Tag = member.User.Id,
            //    HorizontalAlignment = HorizontalAlignment.Stretch
            //};
            //flyoutcontent.Children.Add(Friend);
            flyout.Content = flyoutcontent;
            listviewitem.ContextFlyout = flyout;
            listviewitem.RightTapped += OpenRightTapFlyout;
            listviewitem.Holding += OpenHoldingFlyout;
            #endregion

            return listviewitem;
        }
        private UIElement FriendRender(SharedModels.Friend friend)
        {
            ListViewItem listviewitem = new ListViewItem();
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            Grid image = new Grid();
            image.Height = 48;
            image.Width = 48;
            Rectangle avatar = new Rectangle();
            avatar.RadiusX = 48;
            avatar.RadiusY = 48;
            avatar.Height = 48;
            avatar.Width = 48;
            avatar.HorizontalAlignment = HorizontalAlignment.Left;
            avatar.VerticalAlignment = VerticalAlignment.Top;
            avatar.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + friend.user.Id + "/" + friend.user.Avatar + ".jpg")) };
            image.Children.Add(avatar);
            if (Session.PrecenseDict.ContainsKey(friend.user.Id))
            {
                Rectangle rect = new Rectangle();
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                rect.Height = 12;
                rect.Width = 12;
                rect.HorizontalAlignment = HorizontalAlignment.Right;
                rect.VerticalAlignment = VerticalAlignment.Bottom;

                switch (Session.PrecenseDict[friend.user.Id].Status)
                {
                    case "online":
                        rect.Fill = GetSolidColorBrush("#ff43b581");
                        break;
                    case "idle":
                        rect.Fill = GetSolidColorBrush("#fffaa61a");
                        break;
                    case "offline":
                        rect.Fill = GetSolidColorBrush("#FFAAAAAA");
                        break;
                }

                image.Children.Add(rect);
            }
            else
            {
                Rectangle rect = new Rectangle();
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                rect.Height = 10;
                rect.Width = 10;
                rect.HorizontalAlignment = HorizontalAlignment.Right;
                rect.VerticalAlignment = VerticalAlignment.Bottom;
                rect.Fill = GetSolidColorBrush("#FFAAAAAA");
                image.Children.Add(rect);
            }

            stack.Children.Add(image);

            StackPanel vertstack = new StackPanel();
            TextBlock txtblock = new TextBlock();
            txtblock.FontSize = 13.333;
            txtblock.Margin = new Thickness(12, 0, 0, 0);
            txtblock.Text = friend.user.Username;

            if (friend.user.Bot)
            {
                txtblock.Text += " (BOT)";
            }

            vertstack.Children.Add(txtblock);

            if (Session.PrecenseDict.ContainsKey(friend.user.Id))
            {
                if (Session.PrecenseDict[friend.user.Id].Game != null)
                {
                    TextBlock game = new TextBlock();
                    game.Text = "Playing " + Session.PrecenseDict[friend.user.Id].Game.Value.Name;
                    vertstack.Children.Add(game);
                }
            }
            
            stack.Children.Add(vertstack);
            listviewitem.Content = stack;

            #region Flyout
            Flyout flyout = new Flyout();
            StackPanel flyoutcontent = new StackPanel();
            flyoutcontent.Margin = new Thickness(-10);
            Button profile = new Button()
            {
                Content = "Profile",
                Tag = new Tuple<string, string>((ServerList.SelectedItem as ListViewItem).Tag.ToString(), friend.user.Id),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            profile.Click += OpenProfile;
            flyoutcontent.Children.Add(profile);
            //Button Mention = new Button()
            //{
            //    Content = "Mention",
            //    Tag = "@" + friend.user.Username + "#" + friend.user.Discriminator,
            //    HorizontalAlignment = HorizontalAlignment.Stretch
            //};
            //Mention.Click += AddMention;
            //flyoutcontent.Children.Add(Mention);
            //flyoutcontent.Children.Add(Mention);
            //Button Message = new Button()
            //{
            //    Content = "Message",
            //    Tag = member.User.Id,
            //    HorizontalAlignment = HorizontalAlignment.Stretch
            //};
            //Message.Click += DMessage;
            //flyoutcontent.Children.Add(Message);
            //ToggleButton Mute = new ToggleButton()
            //{
            //    Content = "Mute",
            //    HorizontalAlignment = HorizontalAlignment.Stretch
            //};
            //if (member.Mute)
            //{
            //    Mute.IsChecked = true;
            //}
            //flyoutcontent.Children.Add(Mute);
            //Button Friend = new Button()
            //{
            //    Content = "Add Friend",
            //    Tag = member.User.Id,
            //    HorizontalAlignment = HorizontalAlignment.Stretch
            //};
            //flyoutcontent.Children.Add(Friend);
            flyout.Content = flyoutcontent;
            listviewitem.ContextFlyout = flyout;
            listviewitem.RightTapped += OpenRightTapFlyout;
            listviewitem.Holding += OpenHoldingFlyout;
            #endregion

            return listviewitem;
        }
        private void AddMention(object sender, RoutedEventArgs e)
        {
            if (Storage.Settings.AutoHidePeople)
            {
                Members.IsPaneOpen = false;
            }
            MessageBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string txt);
            MessageBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, txt + " " + (sender as Button).Tag.ToString() + " ");
        }
        private void OpenProfile(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                SharedModels.GuildMember user;
                if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(((sender as Button).Tag as Tuple<string, string>).Item2))
                {
                    user = Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[((sender as Button).Tag as Tuple<string, string>).Item2].Raw;
                }
                else
                {
                    user = Session.GetGuildMember(((sender as Button).Tag as Tuple<string, string>).Item1, ((sender as Button).Tag as Tuple<string, string>).Item2);
                    Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(user.User.Id, new CacheModels.Member(user));
                }

                if (user.User.Id != null && user.User.Avatar != null)
                {
                    PPAvatar.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + user.User.Id + "/" + user.User.Avatar + ".jpg"));
                }
                if (user.User.Username != null)
                {
                    PPUsername.Text = user.User.Username;
                }
                if (user.User.Discriminator != null)
                {

                    PPDiscriminator.Text = "#" + user.User.Discriminator;
                }
                //TODO: Allow and impliment HyperLinkButton
                ProfilePopup.IsPaneOpen = true;
            }
        }

        private UIElement ChannelRender(CacheModels.DmCache channel)
        {

            ListViewItem listviewitem = new ListViewItem();
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            
            Grid image = new Grid();
            Rectangle avatar = new Rectangle();
            avatar.RadiusX = 100;
            avatar.RadiusY = 100;
            avatar.Height = 36;
            avatar.Width = 36;
            avatar.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + channel.Raw.User.Id + "/" + channel.Raw.User.Avatar + ".jpg")) };
            avatar.VerticalAlignment = VerticalAlignment.Center;
            TextBlock txtblock = new TextBlock();
            txtblock.Margin = new Thickness(12, 0, 0, 0);
            txtblock.Text = channel.Raw.User.Username;
            
            txtblock.VerticalAlignment = VerticalAlignment.Center;
            image.Children.Add(avatar);


            if (Session.PrecenseDict.ContainsKey(channel.Raw.User.Id))
            {
                Rectangle rect = new Rectangle();
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                rect.Height = 10;
                rect.Width = 10;
                rect.HorizontalAlignment = HorizontalAlignment.Right;
                rect.VerticalAlignment = VerticalAlignment.Bottom;

                switch (Session.PrecenseDict[channel.Raw.User.Id].Status)
                {
                    case "online":
                        rect.Fill = GetSolidColorBrush("#ff43b581");
                        break;
                    case "idle":
                        rect.Fill = GetSolidColorBrush("#fffaa61a");
                        break;
                    case "offline":
                        rect.Fill = GetSolidColorBrush("#FFAAAAAA");
                        break;
                }

                image.Children.Add(rect);
            }
            else
            {
                Rectangle rect = new Rectangle();
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                rect.Height = 10;
                rect.Width = 10;
                rect.HorizontalAlignment = HorizontalAlignment.Right;
                rect.VerticalAlignment = VerticalAlignment.Bottom;
                rect.Fill = GetSolidColorBrush("#FFAAAAAA");
                image.Children.Add(rect);
            }


            /*if (Storage.MutedChannels.Contains(channel.Raw.Id))
            {
                SolidColorBrush brush = GetSolidColorBrush("#FFFF0000");
                brush.Opacity = 30;
                listviewitem.Background = brush;
            }
            else if (Storage.RecentMessages.ContainsKey(channel.Raw.Id) && Storage.RecentMessages[channel.Raw.Id] != channel.Raw.LastMessageId)
            {
                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                Windows.UI.Color c = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                SolidColorBrush accent = new SolidColorBrush(c);
                accent.Opacity = 30;
                listviewitem.Background = accent;
                listviewitem.Tapped += ClearColor;
            }*/

            stack.Children.Add(image);

            stack.Children.Add(txtblock);
            listviewitem.Content = stack;
            listviewitem.Tag = channel;
            listviewitem.Style = (Style)App.Current.Resources["ChannelItemStyle"];
            listviewitem.Height = 48;
            return listviewitem;
        }

        private UIElement ChannelRender(CacheModels.GuildChannel channel, Permissions perms)
        {
            Permissions chnperms = perms;
            if (channel.Raw.PermissionOverwrites != null)
            {
                chnperms.AddOverwrites(channel.Raw.PermissionOverwrites, channel.Raw.GuildId);
            }

            ListViewItem listviewitem = new ListViewItem();
            
            StackPanel channelcontainer = new StackPanel();
            channelcontainer.Orientation = Orientation.Horizontal;
            TextBlock txtblock = new TextBlock();
            txtblock.Text = "#" + channel.Raw.Name;
            channelcontainer.Opacity = 0.8;
            if (channel.Raw.Type == "text")
            {
                listviewitem.Tag = channel;

                if (Storage.MutedChannels.Contains(channel.Raw.Id))
                {
                    channelcontainer.Children.Add(new SymbolIcon{Symbol = Symbol.Mute, Opacity=0.6, Margin=new Thickness(0,0,4,0)});
                    channelcontainer.Opacity = 0.6;
                }
                else if (Storage.RecentMessages.ContainsKey(channel.Raw.Id) && Storage.RecentMessages[channel.Raw.Id] != channel.Raw.LastMessageId)
                {
                    channelcontainer.Opacity = 1;
                    channelcontainer.Children.Add(new Border{Background=(SolidColorBrush)App.Current.Resources["InvertedBG"], Margin=new Thickness(-14,2,4,0), Height=10, Width=4, CornerRadius = new CornerRadius(0,6,6,0)});
                    listviewitem.Tapped += ClearColor;

                    if (!chnperms.EffectivePerms.ReadMessages && !chnperms.EffectivePerms.Administrator && Storage.Cache.Guilds[channel.Raw.GuildId].RawGuild.OwnerId == Storage.Cache.CurrentUser.Raw.Id)
                    {
                        listviewitem.Visibility = Visibility.Collapsed;
                    }
                }

                #region Flyout
                Flyout flyout = new Flyout();
                StackPanel flyoutcontent = new StackPanel();
                flyoutcontent.Margin = new Thickness(-10);

                if (perms.EffectivePerms.ManageChannels || perms.EffectivePerms.Administrator || Storage.Cache.Guilds[channel.Raw.GuildId].RawGuild.OwnerId == Storage.Cache.CurrentUser.Raw.Id)
                {
                    Button channelSettings = new Button()
                    {
                        Content = "Channel Settings",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Tag = channel.Raw.Id
                    };
                    channelSettings.Click += OpenChannelSettings;
                    flyoutcontent.Children.Add(channelSettings);
                }

                ToggleButton muteChannel = new ToggleButton()
                {
                    Content = "Mute Channel",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = channel.Raw.Id,
                    IsChecked = Storage.MutedChannels.Contains(channel.Raw.Id)
                };

                Button pinToStart = new Button()
                {
                    Content = SecondaryTile.Exists(channel.Raw.Id) ? "Unpin From Start" : "Pin To Start",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = channel
                };
                pinToStart.Click += PinChannelToStart;
                flyoutcontent.Children.Add(pinToStart);

                muteChannel.Click += MuteAChannel;
                flyoutcontent.Children.Add(muteChannel);
                flyout.Content = flyoutcontent;
                listviewitem.ContextFlyout = flyout;
                #endregion

                //return listviewitem;
            }
            else
            {
                //TODO: Support voice channels
            }
            channelcontainer.Children.Add(txtblock);

            listviewitem.Content = channelcontainer;
            return listviewitem;
        }
        private void MuteAChannel(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).Tag != null)
            {
                if (Storage.MutedChannels.Contains((sender as ToggleButton).Tag.ToString()))
                {
                    Storage.MutedChannels.Remove((sender as ToggleButton).Tag.ToString());
                    foreach (ListViewItem item in TextChannels.Items)
                    {
                        if ((item.Tag as CacheModels.GuildChannel).Raw.Id == (sender as ToggleButton).Tag.ToString())
                        {
                            /*if (Storage.RecentMessages.ContainsKey(item.Tag.ToString()) && Storage.RecentMessages[item.Tag.ToString()] != User.messages.First().Id)
                            {
                                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                                Windows.UI.Color c = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                                SolidColorBrush accent = new SolidColorBrush(c);
                                accent.Opacity = Storage.settings.NMIOpacity / 100;
                                item.Background = accent;
                            }
                            else
                            {
                                item.Background = GetSolidColorBrush("#00000000");
                            }*/
                            item.Background = GetSolidColorBrush("#00000000");
                        }
                    }
                }
                else
                {
                    Storage.MutedChannels.Add((sender as ToggleButton).Tag.ToString());
                    foreach (ListViewItem item in TextChannels.Items)
                    {
                        if ((item.Tag as CacheModels.GuildChannel).Raw.Id == (sender as ToggleButton).Tag.ToString())
                        {
                            SolidColorBrush brush = GetSolidColorBrush("#FFFF0000");
                            brush.Opacity = 30;
                            item.Background = brush;
                        }
                    }
                }
                Storage.SaveMutedChannels();
            }
            else if ((sender as AppBarToggleButton).Tag != null)
            {
                if (Storage.MutedChannels.Contains((sender as AppBarToggleButton).Tag.ToString()))
                {
                    Storage.MutedChannels.Remove((sender as AppBarToggleButton).Tag.ToString());
                    foreach (ListViewItem item in TextChannels.Items)
                    {
                        if ((item.Tag as CacheModels.GuildChannel).Raw.Id == (sender as AppBarToggleButton).Tag.ToString())
                        {
                            /*if (Storage.RecentMessages.ContainsKey(item.Tag.ToString()) && Storage.RecentMessages[item.Tag.ToString()] < User.messages.First().Timestamp.Ticks)
                            {
                                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                                Windows.UI.Color c = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                                SolidColorBrush accent = new SolidColorBrush(c);
                                accent.Opacity = Storage.settings.NMIOpacity / 100;
                                item.Background = accent;
                            } else
                            {
                                item.Background = GetSolidColorBrush("#00000000");
                            }*/
                            item.Background = GetSolidColorBrush("#00000000");
                        }
                    }
                }
                else
                {
                    Storage.MutedChannels.Add((sender as AppBarToggleButton).Tag.ToString());
                    foreach (ListViewItem item in TextChannels.Items)
                    {
                        if ((item.Tag as CacheModels.GuildChannel).Raw.Id == (sender as AppBarToggleButton).Tag.ToString())
                        {
                            item.Background = GetSolidColorBrush("#11FF0000");
                        }
                    }
                }
                Storage.SaveMutedChannels();
            }
        }
        private void ClearColor(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                ((sender as ListViewItem).Content as StackPanel).Children.Remove(
                ((sender as ListViewItem).Content as StackPanel).Children.First() as Border);
                ((sender as ListViewItem).Content as StackPanel).Opacity = 0.8;
            }
            catch (Exception){}
        }
        private async void PinChannelToStart(object sender, RoutedEventArgs e)
        {
            if (!SecondaryTile.Exists(((sender as Button).Tag as CacheModels.GuildChannel).Raw.Id))
            {
                var uriLogo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");

                //var currentTime = new DateTime();
                //var tileActivationArguments = "timeTileWasPinned=" + currentTime;
                var tileActivationArguments = ((sender as Button).Tag as CacheModels.GuildChannel).Raw.Id + ":" + ((sender as Button).Tag as CacheModels.GuildChannel).Raw.GuildId;

                var tile = new Windows.UI.StartScreen.SecondaryTile(((sender as Button).Tag as CacheModels.GuildChannel).Raw.Id, ((sender as Button).Tag as CacheModels.GuildChannel).Raw.Name, tileActivationArguments, uriLogo, Windows.UI.StartScreen.TileSize.Default);
                tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                tile.VisualElements.ShowNameOnWide310x150Logo = true;
                tile.VisualElements.ShowNameOnWide310x150Logo = true;

                bool isCreated = await tile.RequestCreateAsync();
                if (isCreated)
                {
                    MessageDialog msg = new MessageDialog("Pinned Succesfully");
                    await msg.ShowAsync();
                    (sender as Button).Content = "Unpin From Start";
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Failed to Pin");
                    await msg.ShowAsync();
                }
            }
            else
            {
                var tileToDelete = new SecondaryTile(((sender as Button).Tag as CacheModels.GuildChannel).Raw.Id);

                bool isDeleted = await tileToDelete.RequestDeleteAsync();
                if (isDeleted)
                {
                    MessageDialog msg = new MessageDialog("Removed Succesfully");
                    await msg.ShowAsync();
                    (sender as Button).Content = "Pin From Start";
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Failed to Remove");
                    await msg.ShowAsync();
                }
            }
        }


        public async void LoadCache()
        {
            try
            {
                StorageFile file = await Storage.SavedData.GetFileAsync("cache");
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CacheModels.TempCache));
                    StringReader messageReader = new StringReader(await FileIO.ReadTextAsync(file));
                    Storage.Cache = new Cache((CacheModels.TempCache)serializer.Deserialize(messageReader));
                }
                catch
                {
                    await file.DeleteAsync();
                    //MessageDialog msg = new MessageDialog("You had a currupted cache, loading was slowed and cache as been reset");
                    //await msg.ShowAsync();
                }
            }
            catch
            {
                MessageDialog msg = new MessageDialog("You had no cache, the app will now start caching data to improve loading times");
                await msg.ShowAsync();
            }
        }
        private async void LoadMessages()
        {
            try
            {
                StorageFile file = await Storage.SavedData.GetFileAsync("messages");
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<ChannelTimeSave>));
                    StringReader messageReader = new StringReader(await FileIO.ReadTextAsync(file));
                    List<ChannelTimeSave> temp = (List<ChannelTimeSave>)serializer.Deserialize(messageReader);
                    foreach (ChannelTimeSave item in temp)
                    {
                        Storage.RecentMessages.Add(item.ChannelId, item.Msgid);
                    }
                }
                catch
                {
                    Storage.RecentMessages.Clear();
                    Storage.SaveMessages();
                    MessageDialog msg = new MessageDialog("There was an issue loading message history saves. History cleared so it can work in the future");
                    msg.ShowAsync();
                }
            }
            catch
            {
                MessageDialog msg = new MessageDialog("You have no history message history saved");
            }
        }
        private async void LoadMutedChannels()
        {
            try
            {
                StorageFile file = await Storage.SavedData.GetFileAsync("mutedchannels");
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                    StringReader messageReader = new StringReader(await FileIO.ReadTextAsync(file));
                    Storage.MutedChannels = (List<string>)serializer.Deserialize(messageReader);
                }
                catch
                {
                    Storage.MutedChannels.Clear();
                    Storage.SaveMutedChannels();
                    MessageDialog msg = new MessageDialog("There was an issue loading muted channels. Cleared so it can work in the future");
                    msg.ShowAsync();
                }
            }
            catch
            {
                MessageDialog msg = new MessageDialog("You have no muted channels saved");
            }
        }

        private void OpenHoldingFlyout(object sender, HoldingRoutedEventArgs e)
        {
            (sender as ListViewItem).ContextFlyout.ShowAt((sender as ListViewItem));
        }
        private void OpenRightTapFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            (sender as ListViewItem).ContextFlyout.ShowAt((sender as ListViewItem));
        }

        #region OldCode
        private UIElement MessageRender(SharedModels.Message message, bool isContinuation, int re)
        {
            Permissions perms = new Permissions();
            if (App.CurrentId != null)
            {
                foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                {
                    if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id))
                    {
                        Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(Storage.Cache.CurrentUser.Raw.Id, new CacheModels.Member(Session.GetGuildMember((ServerList.SelectedItem as ListViewItem).Tag.ToString(), Storage.Cache.CurrentUser.Raw.Id)));
                    }

                    if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Count() != 0 && Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.First().ToString() == role.Id)
                    {
                        perms.GetPermissions(role, Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles);
                    }
                    else
                    {
                        perms.GetPermissions(0);
                    }
                }
            }

            ListViewItem listviewitem = new ListViewItem();
            listviewitem.Style = (Style)App.Current.Resources["MessageStyle"];

            listviewitem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            listviewitem.VerticalAlignment = VerticalAlignment.Stretch;
            listviewitem.Tag = new Nullable<SharedModels.Message>(message);
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(64, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Auto) });
            grid.VerticalAlignment = VerticalAlignment.Stretch;

            //Rectangle avatar = new Rectangle();
            //avatar.Height = 50;
            //avatar.RadiusX = 100;
            //avatar.RadiusY = 100;
            //avatar.VerticalAlignment = VerticalAlignment.Top;
            //avatar.HorizontalAlignment = HorizontalAlignment.Left;
            //ImageBrush imagebrush = new ImageBrush();
            //imagebrush.Stretch = Stretch.Uniform;
            //imagebrush.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + message.User.Id + "/" + message.User.Avatar + ".jpg"));
            //avatar.Fill = imagebrush;
            //grid.Children.Add(avatar);

            if (!isContinuation)
            {
                Rectangle img = new Rectangle();
                img.Height = 48;
                img.Width = 48;
                img.RadiusX = 48;
                img.RadiusY = 48;
                img.VerticalAlignment = VerticalAlignment.Top;
                img.HorizontalAlignment = HorizontalAlignment.Left;
                img.Margin = new Thickness(0, 0, 12, 0);
                if (message.User.Avatar != "" && message.User.Avatar != null)
                {
                    img.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + message.User.Id + "/" + message.User.Avatar + ".jpg")) };
                }
                else
                {
                    img.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png")) };
                }
                grid.Children.Add(img);
            }
            StackPanel innerstack = new StackPanel();
            innerstack.VerticalAlignment = VerticalAlignment.Stretch;
            StackPanel msgData = new StackPanel();
            msgData.Orientation = Orientation.Horizontal;

            if (!isContinuation)
            {
                #region RichTextBlock (user)
                RichTextBlock user = new RichTextBlock();
                user.TextWrapping = TextWrapping.WrapWholeWords;
                Paragraph userPara = new Paragraph();
                Bold boldBlock = new Bold();
                Run run1 = new Run();

                if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() != "DMs")
                {
                    SharedModels.GuildMember member;
                    if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(message.User.Id))
                    {
                        member = Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[message.User.Id].Raw;
                    }
                    else
                    {
                        member = new SharedModels.GuildMember();
                    }
                    if (member.Nick != null)
                    {
                        run1.Text = member.Nick;
                    }
                    else
                    {
                        run1.Text = message.User.Username;
                    }

                    if (member.User.Bot)
                    {
                        run1.Text += " (BOT)";
                    }

                    if (member.Roles != null && member.Roles.Count() > 0)
                    {
                        foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                        {
                            if (role.Id == member.Roles.First<string>())
                            {
                                userPara.Foreground = IntToColor(role.Color);
                            }
                        }
                    }
                }

                boldBlock.Inlines.Add(run1);
                userPara.Inlines.Add(boldBlock);
                user.Blocks.Add(userPara);
                user.Margin = new Thickness(0, 0, 8, 0);
                msgData.Children.Add(user);
                #endregion

                #region RichTextBlock (timestamp)
                RichTextBlock timestamp = new RichTextBlock();
                Paragraph timePara = new Paragraph();
                Run run2 = new Run();
                run2.Text = Common.HumanizeDate(message.Timestamp, null);
                timestamp.Opacity = 0.5;
                timestamp.FontSize = 12;
                timePara.Inlines.Add(run2);
                if (message.EditedTimestamp != null)
                {
                    Run editedrun2 = new Run();
                    editedrun2.Text = " (Edited " + Common.HumanizeDate((DateTime)message.EditedTimestamp, message.Timestamp) + ")";
                    timePara.Inlines.Add(editedrun2);
                }
                timestamp.Blocks.Add(timePara);
                timestamp.Padding = new Thickness(0, 3, 0, 0);

                msgData.Children.Add(timestamp);
                #endregion

            }

            innerstack.Children.Add(msgData);

            #region RichTextBlock
            RichTextBlock txtblock = new RichTextBlock();
            txtblock.TextWrapping = TextWrapping.WrapWholeWords;
            Paragraph txtPara = new Paragraph();

            InlineUIContainer mDcontainer = new InlineUIContainer();
            MarkdownTextBlock.MarkdownTextBlock mdTxtBlock = new MarkdownTextBlock.MarkdownTextBlock();
            mDcontainer.Child = mdTxtBlock;
            mdTxtBlock.Text = message.Content;

            /*if (message.Content == "" )
            {
                run3.Text = "Call";
            }*/
            txtPara.Inlines.Add(mDcontainer);
            mdTxtBlock.FontSize = 13.333;
            mdTxtBlock.Foreground = (SolidColorBrush)App.Current.Resources["MessageForeground"];
            //mdTxtBlock.Link = (SolidColorBrush)App.Current.Resources["LinkColor"];
            txtPara.LineHeight = 18;
            txtblock.Blocks.Add(txtPara);
            if (Session.Online)
            {
                if (message.Embeds != null)
                {
                    foreach (SharedModels.Embed embed in message.Embeds)
                    {
                        Paragraph paragraph = new Paragraph();
                        Hyperlink hyp = new Hyperlink();
                        hyp.Foreground = (SolidColorBrush)App.Current.Resources["LinkColor"];
                        hyp.FontSize = 13.333;
                        if (embed.Url != null)
                        {
                            hyp.NavigateUri = new Uri(embed.Url);
                        }
                        if (embed.title != null)
                        {
                            Run title = new Run();
                            title.Text = embed.title + "\n";
                            hyp.Inlines.Add(title);
                        }
                        if (embed.Description != null)
                        {
                            Run desc = new Run();
                            desc.Text = embed.Description + "\n";
                            hyp.Inlines.Add(desc);
                        }
                        paragraph.Inlines.Add(hyp);
                        if (embed.Thumbnail.ProxyUrl != null)
                        {
                            InlineUIContainer container = new InlineUIContainer();
                            BitmapImage bi = new BitmapImage(new Uri(embed.Thumbnail.ProxyUrl));
                            Image image = new Image();
                            image.Height = 300;
                            image.Source = bi;
                            container.Child = image;
                            paragraph.Inlines.Add(container);
                        }
                        else if (embed.Video.Url != null)
                        {
                            InlineUIContainer container = new InlineUIContainer();
                            MediaElement video = new MediaElement();
                            video.Height = 300;
                            video.Source = (new Uri(embed.Video.Url));
                            container.Child = video;
                            paragraph.Inlines.Add(container);
                        }
                        txtblock.Blocks.Add(paragraph);
                    }
                }

                if (message.Attachments != null)
                {
                    foreach (SharedModels.Attachment attach in message.Attachments)
                    {
                        Paragraph paragraph = new Paragraph();
                        Hyperlink hyp = new Hyperlink();
                        hyp.Foreground = (SolidColorBrush)App.Current.Resources["LinkColor"];
                        hyp.FontSize = 13.333;
                        hyp.NavigateUri = new Uri(attach.Url);
                        Run run = new Run();
                        run.Text = attach.Filename;
                        BitmapImage bi = new BitmapImage(new Uri(attach.Url));
                        Image image = new Image();
                        image.MaxHeight = 300;
                        image.Source = bi;
                        InlineUIContainer container = new InlineUIContainer();
                        container.Child = image;
                        hyp.Inlines.Add(run);
                        paragraph.Inlines.Add(hyp);
                        paragraph.Inlines.Add(new LineBreak());
                        paragraph.Inlines.Add(container);
                        txtblock.Blocks.Add(paragraph);
                    }
                }
            }

            #endregion

            StringBuilder run3Rep = new StringBuilder(mdTxtBlock.Text);
            if (App.CurrentId != null)
            {
                foreach (KeyValuePair<string, CacheModels.GuildChannel> channel in Storage.Cache.Guilds[App.CurrentId].Channels)
                {
                    string channelMention = "<#" + channel.Value.Raw.Id + ">";
                    run3Rep.Replace(channelMention, "#" + channel.Value.Raw.Name);
                }
            }

            foreach (SharedModels.User mention in message.Mentions)
            {
                //HyperlinkButton hyp = new HyperlinkButton();
                //hyp.Tag = mention.Id;
                //hyp.Click += OpenProfile;
                string mentionIntent = "<@" + mention.Id + ">";
                if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() != "DMs")
                {
                    SharedModels.GuildMember member;
                    if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(mention.Id))
                    {
                        member = Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[mention.Id].Raw;
                    }
                    else
                    {
                        member = new SharedModels.GuildMember();
                    }
                    if (member.Nick != null)
                    {

                        run3Rep.Replace(mentionIntent, "@" + member.Nick); ;
                    }
                    else
                    {
                        run3Rep.Replace(mentionIntent, "@" + mention.Username);
                    }
                }
                else
                {
                    run3Rep.Replace(mentionIntent, "@" + mention.Username);
                }
                mentionIntent = "<@!" + mention.Id + ">";
                if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() != "DMs")
                {
                    SharedModels.GuildMember member;
                    if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(mention.Id))
                    {
                        member = Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[mention.Id].Raw;
                    }
                    else
                    {
                        member = new SharedModels.GuildMember();
                    }
                    if (member.Nick != null)
                    {

                        run3Rep.Replace(mentionIntent, "@" + member.Nick); ;
                    }
                    else
                    {
                        run3Rep.Replace(mentionIntent, "@" + mention.Username);
                    }
                }
                else
                {
                    run3Rep.Replace(mentionIntent, "@" + mention.Username);
                }
                if (mention.Id == Storage.Cache.CurrentUser.Raw.Id)
                {
                    listviewitem.Background = GetSolidColorBrush("#19faa61a");
                }
            }

            if (Storage.Settings.HighlightEveryone && (mdTxtBlock.Text.Contains("@everyone") || mdTxtBlock.Text.Contains("@here")))
            {
                listviewitem.Background = GetSolidColorBrush("#19faa61a");
            }

            mdTxtBlock.Text = run3Rep.ToString();
            innerstack.Children.Add(txtblock);

            if (message.Reactions != null)
            {
                GridView gridview = new GridView();
                gridview.SelectionMode = ListViewSelectionMode.None;
                gridview.Margin = new Thickness(0);
                gridview.Padding = new Thickness(0);
                foreach (SharedModels.Reactions reaction in message.Reactions)
                {
                    //GridViewItem gridviewitem = new GridViewItem();
                    ToggleButton gridviewitem = new ToggleButton();
                    gridviewitem.IsChecked = reaction.Me;
                    gridviewitem.Tag = new Tuple<string, string, SharedModels.Reactions>(message.ChannelId, message.Id, reaction);
                    gridviewitem.Click += ToggleReaction;
                    if (reaction.Me)
                    {
                        //var uiSettings = new Windows.UI.ViewManagement.UISettings();
                        //Windows.UI.Color c = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                        //SolidColorBrush accent = new SolidColorBrush(c);
                        //gridviewitem.Background = accent;
                        gridviewitem.IsChecked = true;
                    }

                    StackPanel stack = new StackPanel();
                    stack.Orientation = Orientation.Horizontal;

                    TextBlock textblock = new TextBlock();
                    textblock.Text = reaction.Emoji.Name + " " + reaction.Count.ToString();
                    stack.Children.Add(textblock);

                    gridviewitem.Content = stack;
                    gridviewitem.Style = (Style)App.Current.Resources["EmojiButton"];
                    gridview.Items.Add(gridviewitem);
                }
                innerstack.Children.Add(gridview);
            }

            Grid.SetColumn(innerstack, 1);
            grid.Children.Add(innerstack);

            if (Session.Online)
            {
                #region Flyout
                Button flyoutbutton = new Button();
                flyoutbutton.VerticalAlignment = VerticalAlignment.Stretch;
                flyoutbutton.FontFamily = new FontFamily("Segoe MDL2 Assets");
                flyoutbutton.Content = "";
                flyoutbutton.Style = (Style)App.Current.Resources["IntegratedButton"];
                Grid.SetColumn(flyoutbutton, 2);

                Flyout flyout = new Flyout();
                StackPanel flyoutcontent = new StackPanel();
                flyoutcontent.Margin = new Thickness(-10);
                /*Button AddRection = new Button()
                {
                    Content = "Add Reaction",
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                flyoutcontent.Children.Add(AddRection);*/
                /*Button Pin = new Button()
                {
                    Content = "Pin",
                    Tag = message.Id,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                Pin.Click += PinMessage;
                flyoutcontent.Children.Add(Pin);*/
                Button edit = new Button()
                {
                    Content = "Edit",
                    BorderThickness = new Thickness(0),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = message.Id
                };
                // edit.Click += RenderEdit;
                flyoutcontent.Children.Add(edit);
                Button delete = new Button()
                {
                    Content = "Delete",
                    BorderThickness = new Thickness(0),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = message.Id
                };
                delete.Click += DeleteThisMessage;
                flyoutcontent.Children.Add(delete);
                if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() != "DMs")
                {
                    if (!perms.EffectivePerms.ManageMessages && !perms.EffectivePerms.Administrator && message.User.Id != Storage.Cache.CurrentUser.Raw.Id)
                    {
                        delete.IsEnabled = false;
                    }
                    if (message.User.Id != Storage.Cache.CurrentUser.Raw.Id)
                    {
                        edit.IsEnabled = false;
                    }
                }
                flyout.Content = flyoutcontent;
                flyoutbutton.ContextFlyout = flyout;
                flyoutbutton.Click += OpenFlyout;
                grid.Children.Add(flyoutbutton);
                #endregion
            }

            listviewitem.Content = grid;

            if (isContinuation)
                listviewitem.Padding = new Thickness(12, -14, 12, 16);
            else
                listviewitem.Padding = new Thickness(12, 6, 12, 6);

            return listviewitem;
        }
        private void UpdateEditText(object sender, RoutedEventArgs e)
        {
            (sender as RichEditBox).Document.GetText(Windows.UI.Text.TextGetOptions.None, out Session.Editcache);
        }
        private void SendMessageEdit(object sender, RoutedEventArgs e)
        {
            Session.EditMessage(((sender as Button).Tag as Tuple<string, string>).Item1, ((sender as Button).Tag as Tuple<string, string>).Item2, Session.Editcache);
            LoadChannelMessages(null, null);
        }
        private void OpenFlyout(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextFlyout.ShowAt((sender as Button));
        }
        #endregion
    }
}
