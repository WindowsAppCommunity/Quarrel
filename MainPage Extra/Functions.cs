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
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
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

        private SolidColorBrush IntToColor(int color)
        {
            byte a = (byte)(255);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
        }

        private UIElement MessageRender(SharedModels.Message message)
        {
            Permissions perms = new Permissions();
            if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() != "DMs")
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
            listviewitem.Margin = new Thickness(10,5,10,5);
            listviewitem.Background = GetSolidColorBrush("#FF222222");
            listviewitem.Padding = new Thickness(5);
            listviewitem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            listviewitem.VerticalAlignment = VerticalAlignment.Stretch;
            listviewitem.Tag = new Nullable<SharedModels.Message>(message);
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Auto) });
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

            Rectangle img = new Rectangle();
            img.Height = 50;
            img.Width = 50;
            img.RadiusX = 100;
            img.RadiusY = 100;
            img.VerticalAlignment = VerticalAlignment.Top;
            img.HorizontalAlignment = HorizontalAlignment.Left;
            img.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + message.User.Id + "/" + message.User.Avatar + ".jpg")) };
            grid.Children.Add(img);
            StackPanel innerstack = new StackPanel();
            innerstack.VerticalAlignment = VerticalAlignment.Stretch;
            StackPanel msgData = new StackPanel();
            msgData.Orientation = Orientation.Horizontal;


            #region RichTextBlock
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
            #endregion

            msgData.Children.Add(user);

            #region RichTextBlock
            RichTextBlock timestamp = new RichTextBlock();
            Paragraph timePara = new Paragraph();
            Run run2 = new Run();
            run2.Text = message.Timestamp.Month.ToString() + "/" + message.Timestamp.Day.ToString() + "/" + message.Timestamp.Year.ToString() + " at " + message.Timestamp.Hour.ToString() + ":";
            if (message.Timestamp.Minute < 9)
            {
                run2.Text += "0";
            }
            run2.Text += message.Timestamp.Minute.ToString();
            timestamp.Foreground = GetSolidColorBrush("#FF555555");
            timePara.Inlines.Add(run2);
            if (message.EditedTimestamp != null)
            {
                Run editedrun2 = new Run();
                editedrun2.Text = " (Edited " + message.EditedTimestamp.Value.Month.ToString() + "/" + message.EditedTimestamp.Value.Day.ToString() + "/" + message.EditedTimestamp.Value.Year.ToString() + " at " + message.EditedTimestamp.Value.Hour.ToString() + ":";
                if (message.EditedTimestamp.Value.Minute < 9)
                {
                    editedrun2.Text += "0";
                }
                editedrun2.Text += message.EditedTimestamp.Value.Minute.ToString() + ")";
                timePara.Inlines.Add(editedrun2);
            }
            timestamp.Blocks.Add(timePara);
            timestamp.Margin = new Thickness(5, 0, 0, 0);
            #endregion

            msgData.Children.Add(timestamp);
            innerstack.Children.Add(msgData);

            #region RichTextBlock
            RichTextBlock txtblock = new RichTextBlock();
            txtblock.TextWrapping = TextWrapping.WrapWholeWords;
            Paragraph txtPara = new Paragraph();
            Run run3 = new Run();
            run3.Text = message.Content;
            /*if (message.Content == "" )
            {
                run3.Text = "Call";
            }*/
            txtPara.Inlines.Add(run3);

            txtblock.Blocks.Add(txtPara);

            foreach (SharedModels.Embed embed in message.Embeds)
            {
                Paragraph paragraph = new Paragraph();
                Hyperlink hyp = new Hyperlink();
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
                } else if (embed.Video.Url != null)
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

            foreach (SharedModels.Attachment attach in message.Attachments)
            {
                Paragraph paragraph = new Paragraph();
                Hyperlink hyp = new Hyperlink();
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
            #endregion

            StringBuilder run3Rep = new StringBuilder(run3.Text);
            if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() != "DMs")
            {
                foreach (KeyValuePair<string, CacheModels.GuildChannel> channel in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels)
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
                } else
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
                    listviewitem.Background = GetSolidColorBrush("#55FFAA00");
                }
            }

            if (Storage.Settings.HighlightEveryone && run3.Text.Contains("@everyone"))
            {
                listviewitem.Background = GetSolidColorBrush("#55FFAA00");
            }

            run3.Text = run3Rep.ToString();
            innerstack.Children.Add(txtblock);
            GridView gridview = new GridView();
            gridview.SelectionMode = ListViewSelectionMode.None;
            gridview.Margin = new Thickness(0);
            gridview.Padding = new Thickness(0);

            if (message.Reactions != null)
            {
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
                    gridview.Items.Add(gridviewitem);
                }
            }

            innerstack.Children.Add(gridview);
            Grid.SetColumn(innerstack, 1);
            grid.Children.Add(innerstack);

            #region Flyout
            Button flyoutbutton = new Button();
            flyoutbutton.Background = GetSolidColorBrush("#00FFFFFF");
            flyoutbutton.VerticalAlignment = VerticalAlignment.Stretch;
            flyoutbutton.FontFamily = new FontFamily("Segoe MDL2 Assets");
            flyoutbutton.Content = "";
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
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Tag = message.Id
            };
            edit.Click += RenderEdit;
            flyoutcontent.Children.Add(edit);
            Button delete = new Button()
            {
                Content = "Delete",
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
            flyoutbutton.Click += OpenHoldingFlyout; ;
            grid.Children.Add(flyoutbutton);
            #endregion


            listviewitem.Content = grid;
            return listviewitem;
        }

        private void OpenHoldingFlyout(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextFlyout.ShowAt((sender as Button));
        }

        private void RenderEdit(object sender, RoutedEventArgs e)
        {
            int x = 0;
            foreach (UIElement item in Messages.Children)
            {
                if (item is ListViewItem && (item as ListViewItem).Tag != null && ((item as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id == (sender as Button).Tag.ToString())
                {
                    Messages.Children.RemoveAt(x);
                    if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs")
                    {
                        Messages.Children.Insert(x, EditMessageRender(((item as ListViewItem).Tag as Nullable<SharedModels.Message>).Value));
                    }
                    else
                    {
                        Messages.Children.Insert(x, EditMessageRender(((item as ListViewItem).Tag as Nullable<SharedModels.Message>).Value));
                    }
                }
                x++;
            }
        }

        private UIElement EditMessageRender(SharedModels.Message message)
        {
            ListViewItem item = new ListViewItem();
            item.Margin = new Thickness(10, 5, 10, 5);
            item.Background = GetSolidColorBrush("#FF222222");
            item.Padding = new Thickness(5);
            item.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            item.VerticalAlignment = VerticalAlignment.Stretch;
            item.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            Grid grid = new Grid();
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            RichEditBox txtbox = new RichEditBox();
            txtbox.Document.SetText(Windows.UI.Text.TextSetOptions.None, message.Content);
            txtbox.TextChanged += UpdateEditText;

            Button sendMsg = new Button()
            {
                Content = "",
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                VerticalAlignment = VerticalAlignment.Stretch,
                Tag = new Tuple<string, string>(message.ChannelId, message.Id)
            };

            sendMsg.Click += SendMessageEdit;

            grid.Children.Add(txtbox);

            Grid.SetColumn(sendMsg, 1);
            grid.Children.Add(sendMsg);
            item.Content = grid;
            return item;
        }

        private void UpdateEditText(object sender, RoutedEventArgs e)
        {
             (sender as RichEditBox).Document.GetText(Windows.UI.Text.TextGetOptions.None, out Session.Editcache);
        }

        private void SendMessageEdit(object sender, RoutedEventArgs e)
        {
            Session.EditMessage(((sender as Button).Tag as Tuple<string, string>).Item1, ((sender as Button).Tag as Tuple<string, string>).Item2, Session.Editcache);
        }

        private UIElement GuildRender(CacheModels.Guild guild)
        {
            ListViewItem listviewitem = new ListViewItem();
            //listviewitem.Margin = new Thickness(0, 5, 0 ,5);
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            if (guild.RawGuild.Icon != null)
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(-10, 0, 0, 0);
                Rectangle rect = new Rectangle();
                rect.Height = 45;
                rect.Width = 45;
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
                grid.Margin = new Thickness(-10, 0, 0, 0);
                grid.Height = 45;
                grid.Width = 45;
                Rectangle rect = new Rectangle();
                rect.Fill = GetSolidColorBrush("#FF738AD6");
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                grid.Children.Add(rect);
                TextBlock icon = new TextBlock();
                icon.Text = guild.RawGuild.Name.ToArray<char>()[0].ToString();
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
                Tag = guild.RawGuild.Id
            };
            serverSettings.Click += OpenGuildSettings;
            stack.Children.Add(serverSettings);
            listviewitem.Height = 50;
            listviewitem.Content = stack;
            listviewitem.Tag = guild.RawGuild.Id;
            return listviewitem;
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
            avatar.Height = 50;
            avatar.Width = 50;
            avatar.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + channel.Raw.User.Id + "/" + channel.Raw.User.Avatar + ".jpg")) };
            TextBlock txtblock = new TextBlock();
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
                        rect.Fill = GetSolidColorBrush("#FF00FF00");
                        break;
                    case "idle":
                        rect.Fill = GetSolidColorBrush("#FFFFFF00");
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


            if (Storage.MutedChannels.Contains(channel.Raw.Id))
            {
                SolidColorBrush brush = GetSolidColorBrush("#FFFF0000");
                brush.Opacity = Storage.Settings.NmiOpacity / 100;
                listviewitem.Background = brush;
            }
            else if (Storage.RecentMessages.ContainsKey(channel.Raw.Id) && Storage.RecentMessages[channel.Raw.Id] != channel.Raw.LastMessageId)
            {
                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                Windows.UI.Color c = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                SolidColorBrush accent = new SolidColorBrush(c);
                accent.Opacity = Storage.Settings.NmiOpacity / 100;
                listviewitem.Background = accent;
                listviewitem.Tapped += ClearColor;
            }

            stack.Children.Add(image);

            stack.Children.Add(txtblock);
            listviewitem.Content = stack;
            listviewitem.Tag = channel;
            return listviewitem;
        }

        private UIElement ChannelRender(CacheModels.GuildChannel channel, Permissions perms)
        {
            Permissions chnperms = perms;
            chnperms.AddOverwrites(channel.Raw.PermissionOverwrites, channel.Raw.GuildId);
            ListViewItem listviewitem = new ListViewItem();

            if (!chnperms.EffectivePerms.ReadMessages && !chnperms.EffectivePerms.Administrator && Session.Guild.OwnerId != Storage.Cache.CurrentUser.Raw.Id)
            {
                listviewitem.IsEnabled = false;
                //listviewitem.Visibility = Visibility.Collapsed;
            }

            TextBlock txtblock = new TextBlock();
            txtblock.Text = "#" + channel.Raw.Name;
            if (channel.Raw.Type == "text")
            {
                listviewitem.Content = txtblock;
                listviewitem.Tag = channel;
                if (Storage.MutedChannels.Contains(channel.Raw.Id))
                {
                    SolidColorBrush brush = GetSolidColorBrush("#FFFF0000");
                    brush.Opacity = Storage.Settings.NmiOpacity / 100;
                    listviewitem.Background = brush;
                }
                else if (Storage.RecentMessages.ContainsKey(channel.Raw.Id) && Storage.RecentMessages[channel.Raw.Id] != channel.Raw.LastMessageId)
                {
                    var uiSettings = new Windows.UI.ViewManagement.UISettings();
                    Windows.UI.Color c = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                    SolidColorBrush accent = new SolidColorBrush(c);
                    accent.Opacity = Storage.Settings.NmiOpacity / 100;
                    listviewitem.Background = accent;
                    listviewitem.Tapped += ClearColor;
                }

                #region Flyout
                Flyout flyout = new Flyout();
                StackPanel flyoutcontent = new StackPanel();
                flyoutcontent.Margin = new Thickness(-10);

                if (perms.EffectivePerms.ManageChannels || perms.EffectivePerms.Administrator)
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

                /*Button PinToStart = new Button()
                {
                    Content = SecondaryTile.Exists(channel.Id) ? "Unpin From Start" : "Pin To Start",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = channel
                };
                PinToStart.Click += PinChannelToStart;
                flyoutcontent.Children.Add(PinToStart);*/

                muteChannel.Click += MuteAChannel;
                flyoutcontent.Children.Add(muteChannel);
                flyout.Content = flyoutcontent;
                listviewitem.ContextFlyout = flyout;
                #endregion

                //if (!perms.EffectivePerms.ReadMessages)
                //{
                //    listviewitem.IsEnabled = false;
                //}

                //return listviewitem;
            }
            else
            {
                //TODO: Support voice channels
            }
            return listviewitem;
        }

        private UIElement GuildMemberRender(SharedModels.GuildMember member)
        {
            ListViewItem listviewitem = new ListViewItem();
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            Grid image = new Grid();
            image.Height = 50;
            image.Width = 50;
            Rectangle avatar = new Rectangle();
            avatar.RadiusX = 100;
            avatar.RadiusY = 100;
            avatar.Height = 45;
            avatar.Width = 45;
            avatar.HorizontalAlignment = HorizontalAlignment.Left;
            avatar.VerticalAlignment = VerticalAlignment.Top;
            avatar.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + member.User.Id + "/" + member.User.Avatar + ".jpg")) };
            image.Children.Add(avatar);
            if (Session.PrecenseDict.ContainsKey(member.User.Id))
            {
                Rectangle rect = new Rectangle();
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                rect.Height = 10;
                rect.Width = 10;
                rect.HorizontalAlignment = HorizontalAlignment.Right;
                rect.VerticalAlignment = VerticalAlignment.Bottom;

                switch (Session.PrecenseDict[member.User.Id].Status)
                {
                    case "online":
                        rect.Fill = GetSolidColorBrush("#FF00FF00");
                        break;
                    case "idle":
                        rect.Fill = GetSolidColorBrush("#FFFFFF00");
                        break;
                    case "offline":
                        rect.Fill = GetSolidColorBrush("#FFAAAAAA");
                        break;
                }

                image.Children.Add(rect);
            } else
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

            if (member.Roles != null && member.Roles.Count() > 0)
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
            image.Height = 50;
            image.Width = 50;
            Rectangle avatar = new Rectangle();
            avatar.RadiusX = 100;
            avatar.RadiusY = 100;
            avatar.Height = 45;
            avatar.Width = 45;
            avatar.HorizontalAlignment = HorizontalAlignment.Left;
            avatar.VerticalAlignment = VerticalAlignment.Top;
            avatar.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + friend.user.Id + "/" + friend.user.Avatar + ".jpg")) };
            image.Children.Add(avatar);
            if (Session.PrecenseDict.ContainsKey(friend.user.Id))
            {
                Rectangle rect = new Rectangle();
                rect.RadiusX = 100;
                rect.RadiusY = 100;
                rect.Height = 10;
                rect.Width = 10;
                rect.HorizontalAlignment = HorizontalAlignment.Right;
                rect.VerticalAlignment = VerticalAlignment.Bottom;

                switch (Session.PrecenseDict[friend.user.Id].Status)
                {
                    case "online":
                        rect.Fill = GetSolidColorBrush("#FF00FF00");
                        break;
                    case "idle":
                        rect.Fill = GetSolidColorBrush("#FFFFFF00");
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
                MemberListToggle.IsChecked = false;
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
                    PPAvatar.Source = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + user.User.Id + "/" + user.User.Avatar + ".jpg"));
                }
                if (user.User.Username != null)
                {
                    PPUsername.Text = user.User.Username;
                }
                if (user.User.Discriminator != null)
                {
                    PPDescriminator.Text = "#" + user.User.Discriminator;
                }
                //TODO: Allow and impliment HyperLinkButton
                ProfilePopup.Visibility = Visibility.Visible;
            }
        }

        public async void LoadCache()
        {
            if (Storage.SavedSettings.Containers.ContainsKey("cache"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CacheModels.TempCache));
                    StringReader userNameReader = new StringReader((string)Storage.SavedSettings.Values["cache"]);
                    Storage.Cache = new Cache((CacheModels.TempCache)serializer.Deserialize(userNameReader));
                }
                catch
                {
                    Storage.Cache = new Cache();
                    Storage.SaveCache();
                    MessageDialog msg = new MessageDialog("You had a currupted cache, loading was slowed and cache as been reset");
                    await msg.ShowAsync();
                }
            }
            else
            {
                MessageDialog msg = new MessageDialog("You had no cache, the app will now start caching data to improve loading times");
                await msg.ShowAsync();
            }
        }

        private void LoadSettings()
        {
            if (Storage.SavedSettings.Containers.ContainsKey("settings"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    StringReader userNameReader = new StringReader((string)Storage.SavedSettings.Values["settings"]);
                    Storage.Settings = (Settings)serializer.Deserialize(userNameReader); 
                }
                catch
                {
                    Storage.Settings.AutoHideChannels = true;
                    Storage.Settings.AutoHidePeople = false;
                    Storage.Settings.Toasts = true;
                    Storage.Settings.HighlightEveryone = true;
                    Storage.SaveAppSettings();
                }
            } else
            {
                Storage.Settings.AutoHideChannels = true;
                Storage.Settings.AutoHidePeople = false;
                Storage.Settings.Toasts = true;
                Storage.Settings.HighlightEveryone = true;
                Storage.SaveAppSettings();

                MessageDialog msg = new MessageDialog("You had no settings saved. Defaults set.");
            }
        }

        private void LoadMessages()
        {
            if (Storage.SavedSettings.Containers.ContainsKey("messages"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<ChannelTimeSave>));
                    StringReader messageReader = new StringReader((string)Storage.SavedSettings.Values["messages"]);
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
            else
            {
                MessageDialog msg = new MessageDialog("You have no history message history saved");
            }
        }

        public static ScrollViewer GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            {
                return o as ScrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }


        private void OpenHoldingFlyout(object sender, HoldingRoutedEventArgs e)
        {
            (sender as ListViewItem).ContextFlyout.ShowAt((sender as ListViewItem));
        }
        private void OpenRightTapFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            (sender as ListViewItem).ContextFlyout.ShowAt((sender as ListViewItem));
        }
    }

    class Permissions
    {
        public Permissions()
        {

        }

        public Permissions GetPermissions(SharedModels.Role input, IEnumerable<SharedModels.Role> roles)
        {
            ServerSidePerms = new PermissionsSave(Convert.ToInt64(input.Permissions));

            foreach (SharedModels.Role role in roles)
            {
                if (role.Position <= input.Position)
                {
                    EffectivePerms.AddMerge(Convert.ToInt64(role.Permissions));
                }
            }
            return this;
        }

        public Permissions GetPermissions(long input)
        {
            ServerSidePerms = new PermissionsSave(Convert.ToInt64(input));
            return this;
        }

        public void AddOverwrites(IEnumerable<SharedModels.Overwrite> overwrites, string guild)
        {
            foreach (SharedModels.Overwrite overwrite in overwrites)
            {
                switch (overwrite.Type)
                {
                    case "role":
                        //Find a way to get guild
                        if (Storage.Cache.Guilds[guild].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles != null && (Storage.Cache.Guilds[guild].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id) && Storage.Cache.Guilds[guild].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Contains(overwrite.Id)) || Storage.Cache.Guilds[(guild)].Roles[overwrite.Id].Name == "@everyone")
                        {
                            EffectivePerms.AddMerge(Convert.ToInt64(overwrite.Allow));
                            EffectivePerms.RemoveMerge(Convert.ToInt64(overwrite.Deny));
                        }
                        break;
                    case "member":
                        if (Storage.Cache.CurrentUser.Raw.Id == overwrite.Id)
                        {
                            EffectivePerms.AddMerge(Convert.ToInt64(overwrite.Allow));
                            EffectivePerms.RemoveMerge(Convert.ToInt64(overwrite.Deny));
                        }
                        break;
                }
            }
        }

        public PermissionsSave EffectivePerms;
        public PermissionsSave ServerSidePerms;
    }

    public struct PermissionsSave
    {
        public PermissionsSave(long perms)
        {
            CreateInstantInvite = Convert.ToBoolean(perms & 0x1);
            KickMembers = Convert.ToBoolean(perms & 0x2);
            BanMembers = Convert.ToBoolean(perms & 0x4);
            Administrator = Convert.ToBoolean(perms & 0x8);
            ManageChannels = Convert.ToBoolean(perms & 0x10);
            ManangeGuild = Convert.ToBoolean(perms & 0x20);
            AddReactions = Convert.ToBoolean(perms & 0x40);
            ReadMessages = Convert.ToBoolean(perms & 0x400);
            SendMessages = Convert.ToBoolean(perms & 0x800);
            SendTtsMessages = Convert.ToBoolean(perms & 0x1000);
            ManageMessages = Convert.ToBoolean(perms & 0x2000);
            EmbedLinks = Convert.ToBoolean(perms & 0x4000);
            AttachFiles = Convert.ToBoolean(perms & 0x8000);
            ReadMessageHistory = Convert.ToBoolean(perms & 0x40000);
            MentionEveryone = Convert.ToBoolean(perms & 0x20000);
            UseExternalEmojis = Convert.ToBoolean(perms & 0x40000);
            Connect = Convert.ToBoolean(perms & 0x100000);
            Speak = Convert.ToBoolean(perms & 0x200000);
            MuteMembers = Convert.ToBoolean(perms & 0x400000);
            DeafenMembers = Convert.ToBoolean(perms & 0x800000);
            MoveMembers = Convert.ToBoolean(perms & 0x1000000);
            UseVad = Convert.ToBoolean(perms & 0x2000000);
            ChangeNickname = Convert.ToBoolean(perms & 0x4000000);
            ManageNicknames = Convert.ToBoolean(perms & 0x8000000);
            ManagerWebhooks = Convert.ToBoolean(perms & 0x20000000);
            ManageEmojis = Convert.ToBoolean(perms & 0x40000000);
            ManageRoles = Convert.ToBoolean(perms & 0x10000000);
        }

        public void AddMerge(long perms)
        {
            CreateInstantInvite = CreateInstantInvite ? true : Convert.ToBoolean(perms & 0x1);
            KickMembers = KickMembers ? true : Convert.ToBoolean(perms & 0x2);
            BanMembers = BanMembers ? true : Convert.ToBoolean(perms & 0x4);
            Administrator = Administrator ? true : Convert.ToBoolean(perms & 0x8);
            ManageChannels = ManageChannels ? true : Convert.ToBoolean(perms & 0x10);
            ManangeGuild = ManangeGuild ? true : Convert.ToBoolean(perms & 0x20);
            AddReactions = AddReactions ? true : Convert.ToBoolean(perms & 0x40);
            ReadMessages = ReadMessages ? true : Convert.ToBoolean(perms & 0x400);
            SendMessages = SendMessages ? true : Convert.ToBoolean(perms & 0x800);
            SendTtsMessages = SendTtsMessages ? true : Convert.ToBoolean(perms & 0x1000);
            ManageMessages = ManageMessages ? true : Convert.ToBoolean(perms & 0x2000);
            EmbedLinks = EmbedLinks ? true : Convert.ToBoolean(perms & 0x4000);
            AttachFiles = AttachFiles ? true : Convert.ToBoolean(perms & 0x8000);
            ReadMessageHistory = ReadMessageHistory ? true : Convert.ToBoolean(perms & 0x40000);
            MentionEveryone = MentionEveryone ? true : Convert.ToBoolean(perms & 0x20000);
            UseExternalEmojis = UseExternalEmojis ? true : Convert.ToBoolean(perms & 0x80000);
            Connect = Connect ? true : Convert.ToBoolean(perms & 0x100000);
            Speak = Speak ? true : Convert.ToBoolean(perms & 0x200000);
            MuteMembers = MuteMembers ? true : Convert.ToBoolean(perms & 0x400000);
            DeafenMembers = DeafenMembers ? true : Convert.ToBoolean(perms & 0x800000);
            MoveMembers = MoveMembers ? true : Convert.ToBoolean(perms & 0x1000000);
            UseVad = UseVad ? true : Convert.ToBoolean(perms & 0x2000000);
            ChangeNickname = ChangeNickname ? true :  Convert.ToBoolean(perms & 0x4000000);
            ManageNicknames = ManageNicknames ? true : Convert.ToBoolean(perms & 0x8000000);
            ManagerWebhooks = ManagerWebhooks ? true : Convert.ToBoolean(perms & 0x20000000);
            ManageEmojis = ManageEmojis ? true : Convert.ToBoolean(perms & 0x40000000);
            ManageRoles = ManageRoles ? true : Convert.ToBoolean(perms & 0x10000000);
        }

        public void RemoveMerge(long perms)
        {
            CreateInstantInvite = Convert.ToBoolean(perms & 0x1) ? false : CreateInstantInvite;
            KickMembers = Convert.ToBoolean(perms & 0x2) ? false : KickMembers;
            BanMembers = Convert.ToBoolean(perms & 0x4) ? false : BanMembers;
            Administrator = Convert.ToBoolean(perms & 0x8) ? false : Administrator;
            ManageChannels = Convert.ToBoolean(perms & 0x10) ? false : ManageChannels;
            ManangeGuild = Convert.ToBoolean(perms & 0x20) ? false : ManangeGuild;
            AddReactions = Convert.ToBoolean(perms & 0x40) ? false : AddReactions;
            ReadMessages = Convert.ToBoolean(perms & 0x400) ? false : ReadMessages;
            SendMessages = Convert.ToBoolean(perms & 0x800) ? false : SendMessages;
            SendTtsMessages = Convert.ToBoolean(perms & 0x1000) ? false : SendTtsMessages;
            ManageMessages = Convert.ToBoolean(perms & 0x2000) ? false : ManageMessages;
            EmbedLinks = Convert.ToBoolean(perms & 0x4000) ? false : EmbedLinks;
            AttachFiles = Convert.ToBoolean(perms & 0x8000) ? false : AttachFiles;
            ReadMessageHistory = Convert.ToBoolean(perms & 0x40000) ? false : ReadMessageHistory;
            MentionEveryone = Convert.ToBoolean(perms & 0x20000) ? false : MentionEveryone;
            UseExternalEmojis = Convert.ToBoolean(perms & 0x80000) ? false : UseExternalEmojis;
            Connect = Convert.ToBoolean(perms & 0x100000) ? false : Connect;
            Speak = Convert.ToBoolean(perms & 0x200000) ? false : Speak;
            MuteMembers = Convert.ToBoolean(perms & 0x400000) ? false : MuteMembers;
            DeafenMembers = Convert.ToBoolean(perms & 0x800000) ? false : DeafenMembers;
            MoveMembers = Convert.ToBoolean(perms & 0x1000000) ? false : MoveMembers;
            UseVad = Convert.ToBoolean(perms & 0x2000000) ? false : UseVad;
            ChangeNickname = Convert.ToBoolean(perms & 0x4000000) ? false : ChangeNickname;
            ManageNicknames = Convert.ToBoolean(perms & 0x8000000) ? false : ManageNicknames;
            ManagerWebhooks = Convert.ToBoolean(perms & 0x20000000) ? false : ManagerWebhooks;
            ManageEmojis = Convert.ToBoolean(perms & 0x40000000) ? false : ManageEmojis;
            ManageRoles = Convert.ToBoolean(perms & 0x10000000) ? false : ManageRoles; 
        }

        public bool CreateInstantInvite;
        public bool KickMembers;
        public bool BanMembers;
        public bool Administrator;
        public bool ManageChannels;
        public bool ManangeGuild;
        public bool AddReactions;
        public bool ReadMessages;
        public bool SendMessages;
        public bool SendTtsMessages;
        public bool ManageMessages;
        public bool EmbedLinks;
        public bool AttachFiles;
        public bool ReadMessageHistory;
        public bool MentionEveryone;
        public bool UseExternalEmojis;
        public bool Connect;
        public bool Speak;
        public bool MuteMembers;
        public bool DeafenMembers;
        public bool MoveMembers;
        public bool UseVad;
        public bool ChangeNickname;
        public bool ManageNicknames;
        public bool ManagerWebhooks;
        public bool ManageEmojis;
        public bool ManageRoles;
    }
}
