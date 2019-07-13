using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Quarrel.Messages.Navigation;
using Quarrel.Models.Bindables;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;

namespace Quarrel.ViewModels
{
    public class MessageViewModel : ViewModelBase
    {
        private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ICurrentUsersService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();

        public MessageViewModel()
        {
            Messenger.Default.Register<ChannelNavigateMessage>(this, async m =>
            {
                Channel = m.Channel;

                using (await SourceMutex.LockAsync())
                {
                    NewItemsLoading = true;
                    IEnumerable<Message> itemList = null;
                    //if (Channel.ReadState == null)
                    //    itemList = await ServicesManager.Discord.ChannelService.GetChannelMessages(m.Channel.Model.Id);
                    //else
                    //    itemList = await ServicesManager.Discord.ChannelService.GetChannelMessagesAround(m.Channel.Model.Id, Channel.ReadState.LastMessageId, 50);
                    try
                    {
                        itemList = await discordService.ChannelService.GetChannelMessages(m.Channel.Model.Id, 50);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        return;
                    }

                    await DispatcherHelper.RunAsync(() =>
                    {
                        Source.Clear();

                        Message lastItem = null;

                        BindableMessage scrollItem = null;

                        foreach (Message item in itemList.Reverse())
                        {
                            Source.Add(new BindableMessage(item, guildId, lastItem, lastItem != null && m.Channel.ReadState != null && lastItem.Id == m.Channel.ReadState.LastMessageId));

                            if (lastItem != null && m.Channel.ReadState != null && lastItem.Id == m.Channel.ReadState.LastMessageId)
                            {
                                scrollItem = Source.LastOrDefault();
                            }

                            lastItem = item;
                        }

                        if (scrollItem != null)
                            ScrollTo?.Invoke(this, scrollItem);
                        else
                            ScrollTo?.Invoke(this, Source.LastOrDefault());
                    });
                    NewItemsLoading = false;
                }
            });

            Messenger.Default.Register<GatewayMessageRecievedMessage>(this, async m =>
            {
                if (Channel != null && Channel.Model.Id == m.Message.ChannelId)
                    await DispatcherHelper.RunAsync(() =>
                    {
                        Source.Add(new BindableMessage(m.Message, guildId, Source.LastOrDefault().Model));
                    });
            });

            Messenger.Default.Register<GatewayMessageDeletedMessage>(this, async m => 
            {
                if (Channel != null && Channel.Model.Id == m.ChannelId)
                {
                    await DispatcherHelper.RunAsync(() =>
                    {
                        // LastOrDefault to start from the bottom
                        var msg = Source.LastOrDefault(x => x.Model.Id == m.MessageId);
                        if (msg != null)
                        {
                            Source.Remove(msg);
                        }
                    });
                }
            });

            Messenger.Default.Register<GatewayMessageUpdatedMessage>(this, async m =>
            {
                if (Channel != null && Channel.Model.Id == m.Message.ChannelId)
                {
                    await DispatcherHelper.RunAsync(() =>
                    {
                        // LastOrDefault to start from the bottom
                        var msg = Source.LastOrDefault(x => x.Model.Id == m.Message.Id);
                        if (msg != null)
                        {
                            msg.Update(m.Message);
                        }
                    });
                }
            });
        }

        public event EventHandler<BindableMessage> ScrollTo;

        private string guildId
        {
            get => Channel.GuildId;
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

        private protected AsyncMutex SourceMutex { get; } = new AsyncMutex();

        public bool NewItemsLoading;

        public bool OldItemsLoading;

        private bool ItemsLoading => NewItemsLoading || OldItemsLoading;

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

                    user = currentUsersService.Users.Values.FirstOrDefault(x => x.Model.User.Username == username && x.Model.User.Discriminator == disc).Model.User;

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

            discordService.ChannelService.CreateMessage(Channel.Model.Id, new DiscordAPI.API.Channel.Models.MessageUpsert() { Content = text });
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
                                GuildMember member = currentUsersService.Users.Values
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

        public async void LoadOlderMessages()
        {
            if (ItemsLoading) return;
            using (await SourceMutex.LockAsync())
            {
                OldItemsLoading = true;
                IEnumerable<Message> itemList = await discordService.ChannelService.GetChannelMessagesBefore(Channel.Model.Id, Source.FirstOrDefault().Model.Id);

                await DispatcherHelper.RunAsync(() =>
                {
                    Message lastItem = null;
                    foreach (var item in itemList)
                    {
                        // Can't be last read item
                        Source.Insert(0, new BindableMessage(item, guildId, lastItem));
                        lastItem = item;
                    }
                });
                OldItemsLoading = false;
            }
        }

        public async void LoadNewerMessages()
        {
            if (ItemsLoading) return;
            using (await SourceMutex.LockAsync())
            {
                NewItemsLoading = true;
                if (Channel.Model.LastMessageId != Source.LastOrDefault().Model.Id)
                {
                    IEnumerable<Message> itemList = null;
                    await Task.Run( async () => itemList = await discordService.ChannelService.GetChannelMessagesAfter(Channel.Model.Id, Source.LastOrDefault().Model.Id));

                    await DispatcherHelper.RunAsync(() =>
                    {
                        Message lastItem = null;
                        foreach (var item in itemList)
                        {
                            // Can't be last read item
                            Source.Add(new BindableMessage(item, guildId, lastItem));
                            lastItem = item;
                        }
                    });
                }
                else if (Channel.ReadState == null || Channel.Model.LastMessageId != Channel.ReadState.LastMessageId)
                {
                    await discordService.ChannelService.AckMessage(Channel.Model.Id, Source.LastOrDefault().Model.Id);
                }
                NewItemsLoading = false;
            }
        }
    }
}