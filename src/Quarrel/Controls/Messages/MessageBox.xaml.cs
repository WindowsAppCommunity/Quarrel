using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Models.Bindables;
using Quarrel.Services;
using Quarrel.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using User = DiscordAPI.Models.User;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Messages
{
    public sealed partial class MessageBox : UserControl
    {
        public MessageBox()
        {
            this.InitializeComponent();
        }

        public MessageViewModel ViewModel => DataContext as MessageViewModel;

        #region TextProccessing

        private List<string> FindMentions(string message)
        {
            List<string> mentions = new List<string>();
            bool inMention = false;
            bool inDesc = false;
            bool inChannel = false;
            string cache = "";
            string descCache = "";
            string chnCache = "";
            foreach (char c in message)
            {
                if (inMention)
                {
                    if (c == '#' && !inDesc)
                    {
                        inDesc = true;
                    }
                    else if (c == '@')
                    {
                        inDesc = false;
                        cache = "";
                        descCache = "";
                    }
                    else if (inDesc)
                    {
                        if (Char.IsDigit(c))
                        {
                            descCache += c;
                        }
                        else
                        {
                            inMention = false;
                            inDesc = false;
                            cache = "";
                            descCache = "";
                        }
                        if (descCache.Length == 4)
                        {
                            User mention = null;
                            if (ViewModel.Channel.Model is DirectMessageChannel dmChn)
                            {
                                mention = dmChn.Users
                               .Where(x => x.Username == cache && x.Discriminator == descCache).FirstOrDefault();
                            }
                            else
                            {
                                GuildMember member = Messenger.Default.Request<CurrentMemberListRequestMessage, List<BindableUser>>(new CurrentMemberListRequestMessage())
                               .Where(x => x.Model.User.Username == cache && x.Model.User.Discriminator == descCache).FirstOrDefault().Model;
                                if (member != null)
                                {
                                    mention = member.User;
                                }
                            }
                            if (mention != null)
                            {
                                mentions.Add("@" + cache + "#" + descCache);
                            }
                            inMention = false;
                            inDesc = false;
                            cache = "";
                            descCache = "";
                        }
                    }
                    else
                    {
                        cache += c;
                    }
                }
                else if (inChannel)
                {
                    if (c == ' ')
                    {
                        inChannel = false;
                        chnCache = "";
                    }
                    else
                    {
                        chnCache += c;
                        if (ViewModel.Channel.Model is GuildChannel gChn)
                        {
                            var guild = Messenger.Default.Request<CurrentGuildRequestMessage, BindableGuild>(new CurrentGuildRequestMessage());
                            if (!guild.IsDM)
                            {
                                mentions.Add("#" + chnCache);
                            }
                        }
                    }
                }
                else if (c == '@')
                {
                    inMention = true;
                }
                else if (c == '#')
                {
                    inChannel = true;
                }
            }
            return mentions;
        }

        #endregion

        private string Text
        {
            get => MessageEditor.Text;
            set => MessageEditor.Text = value;
        }

        private void SendBox_OnClick(object sender, RoutedEventArgs e)
        {
            var mentions = FindMentions(Text);
            foreach (var mention in mentions)
            {
                if (mention[0] == '@')
                {
                    int discIndex = mention.IndexOf('#');
                    string username = mention.Substring(1, discIndex - 1);
                    string disc = mention.Substring(1 + discIndex);
                    User user;
                    var userList = Messenger.Default.Request<CurrentMemberListRequestMessage, List<BindableUser>>(new CurrentMemberListRequestMessage());

                    user = userList.FirstOrDefault(x => x.Model.User.Username == username && x.Model.User.Discriminator == disc).Model.User;

                    if (user != null)
                    {
                        Text = Text.Replace("@" + user.Username + "#" + user.Discriminator, "<@!" + user.Id + ">");
                    }
                }
                else if (mention[0] == '#')
                {
                    var guild = Messenger.Default.Request<CurrentGuildRequestMessage, BindableGuild>(new CurrentGuildRequestMessage());
                    if (!guild.IsDM)
                    {
                        var channel = guild.Channels.FirstOrDefault(x => x.Model.Type != 4 && x.Model.Name == mention.Substring(1)).Model;
                        if (channel != null)
                        {
                            Text = Text.Replace("#" + channel.Name, "<#" + channel.Id + ">");
                        }
                    }
                }
            }

            ServicesManager.Discord.ChannelService.CreateMessage(ViewModel.Channel.Model.Id, new DiscordAPI.API.Channel.Models.MessageUpsert() { Content = Text });
            Text = "";
        }

        private void MessageEditor_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                KeyboardCapabilities keyboardCapabilities = new KeyboardCapabilities();
                if (keyboardCapabilities.KeyboardPresent > 0)
                {
                    if (CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                    {
                        InsertNewLine();
                    }
                    else
                    {
                        SendBox_OnClick(null, null);
                    }
                }
                else
                {
                    InsertNewLine();
                }
                e.Handled = true;
            }
        }

        private void InsertNewLine()
        {
            int selectionstart = MessageEditor.SelectionStart;

            if (MessageEditor.SelectionLength > 0)
            {
                // Remove selected text first
                MessageEditor.Text = MessageEditor.Text.Remove(selectionstart, MessageEditor.SelectionLength);
            }

            MessageEditor.Text = MessageEditor.Text.Insert(selectionstart, "\r");
            MessageEditor.SelectionStart = selectionstart + 1;
        }

        private void MessageEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            SendBox.IsEnabled = Text != null;
        }
    }
}
