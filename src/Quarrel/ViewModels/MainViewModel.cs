using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Models.Bindables;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;

namespace Quarrel.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        ICurrentUsersService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();

        public MainViewModel()
        {
            currentUsersService.Users.CollectionChanged += CollectionChangedMethod;

            Messenger.Default.Register<GuildNavigateMessage>(this, async m =>
            {
                if (Guild != m.Guild)
                {
                    await DispatcherHelper.RunAsync(() =>
                    {
                        Guild = m.Guild;
                        BindableChannels.Clear();

                        var itemList = m.Guild.Channels;
                        foreach (var item in itemList)
                        {
                            BindableChannels.Add(item);
                        }
                    });
                }
            });
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
                        BindableMessages.Clear();

                        Message lastItem = null;

                        BindableMessage scrollItem = null;

                        foreach (Message item in itemList.Reverse())
                        {
                            BindableMessages.Add(new BindableMessage(item, guildId, lastItem, lastItem != null && m.Channel.ReadState != null && lastItem.Id == m.Channel.ReadState.LastMessageId));

                            if (lastItem != null && m.Channel.ReadState != null && lastItem.Id == m.Channel.ReadState.LastMessageId)
                            {
                                scrollItem = BindableMessages.LastOrDefault();
                            }

                            lastItem = item;
                        }

                        if (scrollItem != null)
                            ScrollTo?.Invoke(this, scrollItem);
                        else
                            ScrollTo?.Invoke(this, BindableMessages.LastOrDefault());
                    });
                    NewItemsLoading = false;
                }
            });
            Messenger.Default.Register<GatewayMessageRecievedMessage>(this, async m =>
            {
                if (Channel != null && Channel.Model.Id == m.Message.ChannelId)
                    await DispatcherHelper.RunAsync(() =>
                    {
                        BindableMessages.Add(new BindableMessage(m.Message, guildId, BindableMessages.LastOrDefault().Model));
                    });
            });
            Messenger.Default.Register<GatewayMessageDeletedMessage>(this, async m =>
            {
                if (Channel != null && Channel.Model.Id == m.ChannelId)
                {
                    await DispatcherHelper.RunAsync(() =>
                    {
                        // LastOrDefault to start from the bottom
                        var msg = BindableMessages.LastOrDefault(x => x.Model.Id == m.MessageId);
                        if (msg != null)
                        {
                            BindableMessages.Remove(msg);
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
                        var msg = BindableMessages.LastOrDefault(x => x.Model.Id == m.Message.Id);
                        if (msg != null)
                        {
                            msg.Update(m.Message);
                        }
                    });
                }
            });
            Messenger.Default.Register<GatewayReadyMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    #region Settings

                    foreach (var gSettings in m.EventData.GuildSettings)
                    {
                        cacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, gSettings, gSettings.GuildId);

                        foreach (var cSettings in gSettings.ChannelOverrides)
                        {
                            cacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.ChannelSettings, cSettings, cSettings.ChannelId);
                        }
                    }
                    #endregion

                    #region SortReadStates

                    Dictionary<string, ReadState> readStates = new Dictionary<string, ReadState>();
                    foreach (var state in m.EventData.ReadStates)
                    {
                        readStates.Add(state.Id, state);
                    }

                    #endregion

                    #region Guilds and Channels

                    List<BindableGuild> guildList = new List<BindableGuild>();

                    // Add DM
                    var dmGuild = new BindableGuild(new Guild() { Name = "DM", Id = "DM" });
                    guildList.Add(dmGuild);

                    // Add DM channels
                    if (m.EventData.PrivateChannels != null && m.EventData.PrivateChannels.Any())
                    {
                        foreach (var channel in m.EventData.PrivateChannels)
                        {
                            BindableChannel bChannel = new BindableChannel(channel);

                            dmGuild.Channels.Add(bChannel);

                            if (readStates.ContainsKey(bChannel.Model.Id))
                                bChannel.ReadState = readStates[bChannel.Model.Id];

                            _ChannelDictionary.Add(bChannel.Model.Id, bChannel);
                        }

                        // Sort by last message timestamp
                        dmGuild.Channels = dmGuild.Channels.OrderByDescending(x => Convert.ToUInt64(x.Model.LastMessageId)).ToList();
                    }


                    foreach (var guild in m.EventData.Guilds)
                    {
                        BindableGuild bGuild = new BindableGuild(guild);

                        // Handle guild settings
                        GuildSetting gSettings = cacheService.Runtime.TryGetValue<GuildSetting>(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, guild.Id);
                        if (gSettings != null)
                        {
                            bGuild.Muted = gSettings.Muted;
                        }

                        // Guild Order
                        bGuild.Position = m.EventData.Settings.GuildOrder.IndexOf(x => x == bGuild.Model.Id);

                        // Guild Channels
                        foreach (var channel in guild.Channels)
                        {
                            IEnumerable<VoiceState> state = guild.VoiceStates?.Where(x => x.ChannelId == channel.Id);
                            BindableChannel bChannel = new BindableChannel(channel, state);

                            bChannel.GuildId = guild.Id;

                            // Handle channel settings
                            ChannelOverride cSettings = cacheService.Runtime.TryGetValue<ChannelOverride>(Quarrel.Helpers.Constants.Cache.Keys.ChannelSettings, channel.Id);
                            if (cSettings != null)
                            {
                                bChannel.Muted = cSettings.Muted;
                            }

                            // Find parent position
                            if (!string.IsNullOrEmpty(bChannel.ParentId))
                                bChannel.ParentPostion = guild.Channels.First(x => x.Id == bChannel.ParentId).Position;

                            if (readStates.ContainsKey(bChannel.Model.Id))
                                bChannel.ReadState = readStates[bChannel.Model.Id];

                            bGuild.Channels.Add(bChannel);
                            _ChannelDictionary.Add(bChannel.Model.Id, bChannel);
                        }

                        bGuild.Channels = bGuild.Channels.OrderBy(x => x.AbsolutePostion).ToList();

                        bGuild.Model.Channels = null;


                        //foreach (var user in guild.Members)
                        //{
                        //    BindableUser bgMember = new BindableUser(user);
                        //    bgMember.GuildId = guild.Id;
                        //    // TODO: Add to Memberlist
                        //}

                        // Guild Roles
                        foreach (var role in guild.Roles)
                        {
                            cacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildRole, role, guild.Id + role.Id);
                        }

                        // Guild Presences
                        foreach (var presence in guild.Presences)
                        {
                            cacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Presence, presence, presence.User.Id);
                        }

                        guildList.Add(bGuild);
                    }

                    guildList = guildList.OrderBy(x => x.Position).ToList();
                    #endregion

                    // Show guilds
                    foreach (var guild in guildList)
                    {
                        BindableGuilds.Add(guild.Model.Id, guild);
                    }
                });
            });

            Messenger.Default.Register<BindableGuildRequestMessage>(this, m => m.ReportResult(BindableGuilds[m.GuildId]));
            Messenger.Default.Register<BindableChannelRequestMessage>(this, m => m.ReportResult(_ChannelDictionary[m.ChannelId]));
            Messenger.Default.Register<CurrentGuildRequestMessage>(this, m => m.ReportResult(Guild));
        }


        Dictionary<string, BindableChannel> _ChannelDictionary = new Dictionary<string, BindableChannel>();


        public event EventHandler<BindableMessage> ScrollTo;

        private string guildId
        {
            get => Channel.GuildId;
        }

        private protected AsyncMutex SourceMutex { get; } = new AsyncMutex();

        public bool NewItemsLoading;

        public bool OldItemsLoading;

        private bool ItemsLoading => NewItemsLoading || OldItemsLoading;

        #region Commands
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

        private RelayCommand<BindableGuild> navigateGuildCommand;
        public RelayCommand<BindableGuild> NavigateGuildCommand => navigateGuildCommand ?? (navigateGuildCommand = new RelayCommand<BindableGuild>((guild) =>
        {
            Messenger.Default.Send(new GuildNavigateMessage(guild));
        }));

        private RelayCommand<BindableChannel> navigateChannelCommand;
        public RelayCommand<BindableChannel> NavigateChannelCommand => navigateChannelCommand ?? (navigateChannelCommand = new RelayCommand<BindableChannel>(async (channel) =>
        {
            if (channel.IsCategory)
            {
                bool newState = !channel.Collapsed;
                for (int i = BindableChannels.IndexOf(channel);
                    i < BindableChannels.Count
                    && (BindableChannels[i] is BindableChannel bChannel)
                    && bChannel.ParentId == channel.Model.Id;
                    i++)
                {
                    bChannel.Collapsed = newState;
                }
            }
            else if (channel.IsVoiceChannel)
            {
                if (channel.Model is GuildChannel gChannel)
                    await discordService.Gateway.Gateway.VoiceStatusUpdate(Guild.Model.Id, gChannel.Id, false, false);
            }
            else
            {
                Messenger.Default.Send(new ChannelNavigateMessage(channel, Guild));
            }
        }));
        #endregion

        #region Methods
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

        public async void LoadOlderMessages()
        {
            if (ItemsLoading) return;
            using (await SourceMutex.LockAsync())
            {
                OldItemsLoading = true;
                IEnumerable<Message> itemList = await discordService.ChannelService.GetChannelMessagesBefore(Channel.Model.Id, BindableMessages.FirstOrDefault().Model.Id);

                await DispatcherHelper.RunAsync(() =>
                {
                    Message lastItem = null;
                    foreach (var item in itemList)
                    {
                        // Can't be last read item
                        BindableMessages.Insert(0, new BindableMessage(item, guildId, lastItem));
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
                if (Channel.Model.LastMessageId != BindableMessages.LastOrDefault().Model.Id)
                {
                    IEnumerable<Message> itemList = null;
                    await Task.Run(async () => itemList = await discordService.ChannelService.GetChannelMessagesAfter(Channel.Model.Id, BindableMessages.LastOrDefault().Model.Id));

                    await DispatcherHelper.RunAsync(() =>
                    {
                        Message lastItem = null;
                        foreach (var item in itemList)
                        {
                            // Can't be last read item
                            BindableMessages.Add(new BindableMessage(item, guildId, lastItem));
                            lastItem = item;
                        }
                    });
                }
                else if (Channel.ReadState == null || Channel.Model.LastMessageId != Channel.ReadState.LastMessageId)
                {
                    await discordService.ChannelService.AckMessage(Channel.Model.Id, BindableMessages.LastOrDefault().Model.Id);
                }
                NewItemsLoading = false;
            }
        }
        private void CollectionChangedMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        _ = DispatcherHelper.RunAsync(() =>
                        {
                            foreach (var item in e.NewItems)
                            {
                                if (item is KeyValuePair<string, BindableGuildMember> member)
                                {
                                    BindableMembers.AddElement(member.Value);
                                }
                            }

                        });

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        _ = DispatcherHelper.RunAsync(() =>
                        {
                            foreach (var item in e.OldItems)
                            {
                                if (item is KeyValuePair<string, BindableGuildMember> member)
                                {
                                    BindableMembers.RemoveElement(member.Value);
                                }
                            }

                            foreach (var item in e.NewItems)
                            {
                                if (item is KeyValuePair<string, BindableGuildMember> member)
                                {
                                    BindableMembers.AddElement(member.Value);
                                }
                            }
                        });

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        _ = DispatcherHelper.RunAsync(() =>
                        {
                            foreach (var item in e.OldItems)
                            {
                                if (item is BindableGuildMember member)
                                {
                                    BindableMembers.RemoveElement(member);
                                }
                            }
                        });

                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        _ = DispatcherHelper.RunAsync(() =>
                        {
                            //Note: reset must only be called from clear or this will not work

                            BindableMembers.Clear();
                        });

                        break;
                    }
            }
        }

        #endregion

        #region Properties
        private BindableGuild _Guild;
        public BindableGuild Guild
        {
            get => _Guild;
            set => Set(ref _Guild, value);
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
        [NotNull]
        public ObservableHashedCollection<string, BindableGuild> BindableGuilds { get; private set; } = new ObservableHashedCollection<string, BindableGuild>(new List<KeyValuePair<string, BindableGuild>>());
        /// <summary>
        /// Gets the collection of grouped feeds to display
        /// </summary>
        [NotNull]
        public ObservableCollection<BindableMessage> BindableMessages { get; private set; } = new ObservableCollection<BindableMessage>();
        [NotNull]
        public ObservableCollection<BindableChannel> BindableChannels { get; private set; } = new ObservableCollection<BindableChannel>();
        [NotNull]
        public ObservableSortedGroupedCollection<Role, BindableGuildMember> BindableMembers { get; set; } = new ObservableSortedGroupedCollection<Role, BindableGuildMember>(x => x.TopHoistRole, x => -x.Position);
        #endregion
    }
}
