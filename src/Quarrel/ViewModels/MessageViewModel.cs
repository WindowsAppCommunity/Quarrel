using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Messages.Navigation;
using Quarrel.Models.Bindables;
using UICompositionAnimations.Helpers;
using System.Collections;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services;

namespace Quarrel.ViewModels
{
    public class MessageViewModel : ViewModelBase
    {
        public MessageViewModel()
        {
            Messenger.Default.Register<ChannelNavigateMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(async () =>
                {
                    Channel = m.Channel;

                    Source.Clear();
                    var itemList = await Services.ServicesManager.Discord.ChannelService.GetChannelMessages(m.Channel.Model.Id);

                    Message lastItem = null;

                    foreach (Message item in itemList.Reverse())
                    {
                        Source.Add(new BindableMessage(item, guildId, lastItem));
                        lastItem = item;
                    }
                });
            });

            Messenger.Default.Register<GatewayMessageRecievedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    if (Channel != null && Channel.Model.Id == m.Message.ChannelId)
                        Source.Add(new BindableMessage(m.Message, guildId, Source.LastOrDefault().Model));
                });
            });
        }

        private string guildId
        {
            get => Channel.Model is GuildChannel gChn ? gChn.GuildId : "DM";
        }

        private BindableChannel _Channel;

        public BindableChannel Channel
        {
            get => _Channel;
            set => Set(ref _Channel, value);
        }

        private string _MessageText = "";

        public string MessageText
        {
            get => _MessageText;
            set => Set(ref _MessageText, value);
        }

        public ObservableCollection<BindableMessage> Source { get; private set; } = new ObservableCollection<BindableMessage>();

        private RelayCommand sendMessageCommand;

        public RelayCommand SendMessageCommand => sendMessageCommand ?? (sendMessageCommand = new RelayCommand(() =>
        {
            string text = MessageText;
            var mentions = FindMentions(text);
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
                        text = text.Replace("@" + user.Username + "#" + user.Discriminator, "<@!" + user.Id + ">");
                    }
                }
                else if (mention[0] == '#')
                {
                    var guild = Messenger.Default.Request<CurrentGuildRequestMessage, BindableGuild>(new CurrentGuildRequestMessage());
                    if (!guild.IsDM)
                    {
                        var channel = guild.Channels.FirstOrDefault(x => x.Model.Type != 4 && x.Model.Name == mention.Substring(1)).Model;
                        text = text.Replace("#" + channel.Name, "<#" + channel.Id + ">");
                    }
                }
            }

            ServicesManager.Discord.ChannelService.CreateMessage(Channel.Model.Id, new DiscordAPI.API.Channel.Models.MessageUpsert() { Content = text });
            MessageText = "";
        }));


        private RelayCommand newLineCommand;

        public RelayCommand NewLineCommand =>
            newLineCommand ?? (newLineCommand = new RelayCommand(() =>
            {
                string text = MessageText;
                int selectionstart = SelectionStart;

                if (SelectionLength > 0)
                {
                    // Remove selected text first
                    text = text.Remove(selectionstart, SelectionLength);
                }

                text = text.Insert(selectionstart, Environment.NewLine + Environment.NewLine); //Not sure why two lines breaks are needed but it doesn't work otherwise
                MessageText = text;
                SelectionStart = selectionstart + 1;
            }));
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
                        if (char.IsDigit(c))
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
                            User mention;
                            if (Channel.Model is DirectMessageChannel dmChn)
                            {
                                mention = dmChn.Users.FirstOrDefault(x => x.Username == cache && x.Discriminator == descCache);
                            }
                            else
                            {
                                GuildMember member = Messenger.Default
                                           .Request<CurrentMemberListRequestMessage, List<BindableUser>>(new CurrentMemberListRequestMessage())
                                           .FirstOrDefault(x => x.Model.User.Username == cache && x.Model.User.Discriminator == descCache).Model;
                                mention = member.User;
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
                        if (Channel.Model is GuildChannel)
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


        private int _SelectionStart;

        public int SelectionStart
        {
            get => _SelectionStart;
            set => Set(ref _SelectionStart, value);
        }

        private int _SelectionLength;

        public int SelectionLength
        {
            get => _SelectionLength;
            set => Set(ref _SelectionLength, value);
        }

    }
}