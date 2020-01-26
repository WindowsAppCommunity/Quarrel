using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Quarrel.ViewModels
{
    public partial class MainViewModel
    {
        #region Events

        /// <summary>
        /// Scrolls message list to BindableMessage
        /// </summary>
        public event EventHandler<BindableMessage> ScrollTo;

        #endregion

        #region Methods

        #region Initialize

        private void RegisterMessagesMessages()
        {
            #region Gateway

            #region Messages

            // Handles incoming messages
            MessengerInstance.Register<GatewayMessageRecievedMessage>(this, m =>
            {
                // Check if channel exists
                if (ChannelsService.AllChannels.TryGetValue(m.Message.ChannelId, out BindableChannel channel))
                {
                    channel.UpdateLMID(m.Message.Id);

                    // Updates Mention count
                    if (channel.IsDirectChannel || channel.IsGroupChannel ||
                        m.Message.Mentions.Any(x => x.Id == CurrentUserService.CurrentUser.Model.Id) ||
                        m.Message.MentionEveryone)
                        DispatcherHelper.CheckBeginInvokeOnUi(() =>
                        {
                            channel.ReadState.MentionCount++;
                            if (channel.IsDirectChannel || channel.IsGroupChannel)
                            {
                                int oldIndex = GuildsService.AllGuilds["DM"].Channels.IndexOf(channel);
                                if (oldIndex >= 0)
                                    GuildsService.AllGuilds["DM"].Channels.Move(oldIndex, 0);
                            }
                        });

                    // Removes typer from Channel if responsible for sending this message
                    if (CurrentChannel != null && CurrentChannel.Model.Id == channel.Model.Id)
                        DispatcherHelper.CheckBeginInvokeOnUi(() =>
                        {
                            channel.Typers.TryRemove(m.Message.User.Id, out _);
                            BindableMessage lastMessage = BindableMessages.LastOrDefault();
                            BindableMessages.Add(new BindableMessage(m.Message, channel.Guild.Model.Id ?? "DM",
                                BindableMessages.LastOrDefault().Model.User != null && BindableMessages.LastOrDefault().Model.User.Id == m.Message.User.Id));
                        });
                }
            });

            // Handles message deletion
            MessengerInstance.Register<GatewayMessageDeletedMessage>(this, m =>
            {
                if (CurrentChannel != null && CurrentChannel.Model.Id == m.ChannelId)
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        BindableMessage msg = BindableMessages.LastOrDefault(x => x.Model.Id == m.MessageId);
                        if (msg != null)
                        {
                            int index = BindableMessages.IndexOf(msg);
                            if (!msg.IsContinuation && index != BindableMessages.Count - 1)
                                BindableMessages[index + 1].IsContinuation = false;
                            BindableMessages.Remove(msg);
                        }
                    });
            });

            // Handles message updated
            MessengerInstance.Register<GatewayMessageUpdatedMessage>(this, m =>
            {
                if (CurrentChannel != null && CurrentChannel.Model.Id == m.Message.ChannelId)
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        BindableMessage msg = BindableMessages.LastOrDefault(x => x.Model.Id != "Ad");
                        msg?.Update(m.Message);
                    });
            });

            #endregion

            #region Reactions

            MessengerInstance.Register<GatewayReactionAddedMessage>(this, m =>
            {
                if (CurrentChannel != null && CurrentChannel.Model.Id != m.ChannelId)
                    return;

                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    BindableMessage message = BindableMessages.LastOrDefault(x => m.MessageId == x.Model.Id);
                    if (message != null)
                    {
                        BindableReaction reaction = message.BindableReactions.FirstOrDefault(x =>
                            x.Model.Emoji.Name == m.Emoji.Name && x.Model.Emoji.Id == m.Emoji.Id);
                        if (reaction != null)
                        {
                            reaction.Count++;
                        }
                        else
                        {
                            reaction = new BindableReaction(new Reaction() { Emoji = m.Emoji, Count = 1, Me = false });
                            message.BindableReactions.Add(reaction);
                        }
                    }
                });
            });
            MessengerInstance.Register<GatewayReactionRemovedMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    BindableMessage message = BindableMessages.LastOrDefault(x => x.Model.Id == m.MessageId);
                    if (message != null)
                    {
                        BindableReaction reaction = message.BindableReactions?.FirstOrDefault(x =>
                            x.Model.Emoji.Name == m.Emoji.Name && x.Model.Emoji.Id == m.Emoji.Id);
                        if (reaction != null)
                        {
                            reaction.Count--;
                            if (reaction.Count == 0)
                            {
                                message.BindableReactions.Remove(reaction);
                            }
                        }
                    }
                });
            });

            #endregion

            #endregion
        }

        #endregion

        #region Message Drafting

        /// <summary>
        /// Replaces surrogates with proper values for Emojis and Mentions
        /// </summary>
        /// <returns>Reformatted message string</returns>
        private string ReplaceMessageDraftSurrogates()
        {
            string formattedMessage = MessageText;

            // Emoji surrogates
            var emojiMatches = Regex.Matches(formattedMessage, Constants.Regex.EmojiSurrogateRegex);
            foreach (Match match in emojiMatches)
            {
                // Finds emoji by name
                Emoji emoji = CurrentGuild.Model.Emojis.FirstOrDefault(x => x.Name == match.Groups[1].Value);

                // Replaces :emoji_name: format with <emoji_name:id> format
                if (emoji != null)
                {
                    // Different format if animated
                    string format = emoji.Animated ? "<a:{0}:{1}>" : "<:{0}:{1}>";
                    formattedMessage = formattedMessage.Replace(match.Value, string.Format(format, emoji.Name, emoji.Id));
                }
            }

            // User mentions
            var userMentionMatches = Regex.Matches(formattedMessage, Constants.Regex.UserMentionSurrogateRegex);
            foreach (Match match in userMentionMatches)
            {
                // Finds user from Username and Discriminator
                BindableGuildMember user = CurrentBindableMembers
                    .FirstOrDefault(x => (x is BindableGuildMember bmx) &&
                    bmx.Model.User.Username == match.Groups[1].Value &&
                    bmx.Model.User.Discriminator == match.Groups[2].Value) as BindableGuildMember;

                // Replaces @name#disc format with <@!ID> format
                if (user != null)
                {
                    formattedMessage = formattedMessage.Replace(match.Value, string.Format("<@!{0}>", user.Model.User.Id));
                }
            }

            // Channel Mentions
            if (!CurrentGuild.IsDM)
            {
                var channelMentionMatches = Regex.Matches(formattedMessage, Constants.Regex.ChannelMentionSurrogateRegex);
                foreach (Match match in channelMentionMatches)
                {
                    // Finds channel by name, in current guild
                    BindableChannel channel = GuildsService.CurrentGuild.Channels.FirstOrDefault(x => x.Model.Name == match.Groups[1].Value);

                    // Replaces #channel-name
                    if (channel != null)
                    {
                        formattedMessage = formattedMessage.Replace(match.Value, string.Format("<#{0}>", channel.Model.Id));
                    }
                }
            }


            return formattedMessage;
        }

        #endregion

        #region Message Loading

        /// <summary>
        /// Loads messages from before the first message in the message list
        /// </summary>
        public async void LoadOlderMessages()
        {
            if (ItemsLoading || _AtTop) return;
            await SemaphoreSlim.WaitAsync();
            try
            {
                OldItemsLoading = true;
                IEnumerable<Message> itemList =
                    await DiscordService.ChannelService.GetChannelMessagesBefore(CurrentChannel.Model.Id,
                        BindableMessages.FirstOrDefault(x => x.Model.Id != "Ad").Model.Id);

                List<BindableMessage> messages = new List<BindableMessage>();
                Message lastItem = null;

                if (itemList.Count() < 50)
                {
                    _AtTop = true;
                    if (!itemList.Any())
                    {
                        OldItemsLoading = false;
                        return;
                    }
                }


                IReadOnlyDictionary<string, GuildMember> guildMembers = guildId != "DM"
                    ? GuildsService.GetAndRequestGuildMembers(itemList.Select(x => x.User.Id).Distinct(),
                        guildId)
                    : null;


                for (int i = itemList.Count() - 1; i >= 0; i--)
                {
                    Message item = itemList.ElementAt(i);

                    // Can't be last read item
                    messages.Add(new BindableMessage(item, guildId,
                        lastItem != null && lastItem.User.Id == item.User.Id,
                        false,
                        guildMembers != null && guildMembers.TryGetValue(item.User.Id, out GuildMember member)
                            ? member
                            : null));
                    lastItem = item;

                    if (!SettingsService.Roaming.GetValue<bool>(SettingKeys.AdsRemoved) && i % 10 == 0)
                    {
                        messages.Add(new BindableMessage(new Message() { Id = "Ad", ChannelId = CurrentChannel.Model.Id },
                            null));
                        lastItem = null;
                    }
                }

                if (messages.Count > 0)
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        BindableMessages.InsertRange(0, messages, NotifyCollectionChangedAction.Reset);
                        OldItemsLoading = false;
                    });
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Loads messages from after the last message in the message list
        /// </summary>
        public async void LoadNewerMessages()
        {
            if (ItemsLoading) return;
            await SemaphoreSlim.WaitAsync();
            try
            {
                NewItemsLoading = true;
                if (CurrentChannel.Model.LastMessageId != BindableMessages.LastOrDefault(x => x.Model.Id != "Ad").Model.Id)
                {
                    IEnumerable<Message> itemList = null;
                    await Task.Run(async () =>
                        itemList = await DiscordService.ChannelService.GetChannelMessagesAfter(CurrentChannel.Model.Id,
                            BindableMessages.LastOrDefault(x => x.Model.Id != "Ad").Model.Id));

                    List<BindableMessage> messages = new List<BindableMessage>();
                    Message lastItem = null;

                    for (int i = 0; i < itemList.Count(); i++)
                    {
                        Message item = itemList.ElementAt(i);

                        // Can't be last read item
                        messages.Add(new BindableMessage(item, guildId));
                        lastItem = item;

                        if (!SettingsService.Roaming.GetValue<bool>(SettingKeys.AdsRemoved) && i % 10 == 0)
                        {
                            messages.Add(new BindableMessage(new Message() { Id = "Ad", ChannelId = CurrentChannel.Model.Id },
                                null));
                            lastItem = null;
                        }
                    }

                    if (messages.Count > 0)
                        DispatcherHelper.CheckBeginInvokeOnUi(() =>
                        {
                            BindableMessages.AddRange(messages, NotifyCollectionChangedAction.Reset);
                        });
                }
                else if (CurrentChannel.ReadState == null || CurrentChannel.Model.LastMessageId != CurrentChannel.ReadState.LastMessageId)
                {
                    await DiscordService.ChannelService.AckMessage(CurrentChannel.Model.Id,
                        BindableMessages.LastOrDefault(x => x.Model.Id != "Ad").Model.Id);
                }

                NewItemsLoading = false;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region Message Drafting

        /// <summary>
        /// Sends API message to indicate typing state
        /// </summary>
        public RelayCommand TriggerTyping => tiggerTyping ??= new RelayCommand(() =>
        {
            DiscordService.ChannelService.TriggerTypingIndicator(CurrentChannel.Model.Id);
        });
        private RelayCommand tiggerTyping;

        /// <summary>
        /// Handles enter override on MessageBox to add new line
        /// </summary>
        public RelayCommand NewLineCommand =>
            newLineCommand ??= new RelayCommand(() =>
            {
                string text = MessageText;
                int selectionstart = SelectionStart;

                if (SelectionLength > 0)
                    // Remove selected text first
                    text = text.Remove(selectionstart, SelectionLength);

                text = text.Insert(selectionstart, " \n");
                MessageText = text;
                SelectionStart = selectionstart + 2;
            });
        private RelayCommand newLineCommand;

        /// <summary>
        /// Handles enter override on MessageBox to send message
        /// </summary>
        public RelayCommand SendMessageCommand => sendMessageCommand ??= new RelayCommand(async () =>
        {
            string text = ReplaceMessageDraftSurrogates();

            await DiscordService.ChannelService.CreateMessage(CurrentChannel.Model.Id,
                new DiscordAPI.API.Channel.Models.MessageUpsert() { Content = text });
            DispatcherHelper.CheckBeginInvokeOnUi(() => { MessageText = ""; });
        });
        private RelayCommand sendMessageCommand;

        #endregion

        #region Message Editing

        /// <summary>
        /// Override up arrow to edit last sent message in chat
        /// </summary>
        public RelayCommand EditLastMessageCommand => editLastMessageCommand ??= new RelayCommand(() =>
        {
            // Only overrides if there's no draft
            if (string.IsNullOrEmpty(MessageText))
            {
                var userLastMessage = BindableMessages.LastOrDefault(x => x.Model.Id != "Ad" && x.Model.User.Id == CurrentUserService.CurrentUser.Model.Id);
                if (userLastMessage != null)
                {
                    userLastMessage.IsEditing = true;
                    ScrollTo?.Invoke(this, userLastMessage);
                }
            }
        });
        private RelayCommand editLastMessageCommand;

        /// <summary>
        /// Sends API request to delete a message
        /// </summary>
        public RelayCommand<BindableMessage> DeleteMessageCommand => deleteMessageCommand ??=
            new RelayCommand<BindableMessage>(async (message) =>
            {
                await DiscordService.ChannelService.DeleteMessage(message.Model.ChannelId, message.Model.Id);
            });
        private RelayCommand<BindableMessage> deleteMessageCommand;

        /// <summary>
        /// Sends API request to pin a message
        /// </summary>
        public RelayCommand<BindableMessage> PinMessageCommand => pinMessageCommand ??=
            new RelayCommand<BindableMessage>(async (message) =>
            {
                await DiscordService.ChannelService.AddPinnedChannelMessage(message.Model.ChannelId,
                    message.Model.Id);
            });
        private RelayCommand<BindableMessage> pinMessageCommand;

        /// <summary>
        /// Sends API request to unpin a message
        /// </summary>
        public RelayCommand<BindableMessage> UnPinMessageCommand => unPinMessageCommand ??=
            new RelayCommand<BindableMessage>(async (message) =>
            {
                await DiscordService.ChannelService.DeletePinnedChannelMessage(message.Model.ChannelId,
                    message.Model.Id);
            });
        private RelayCommand<BindableMessage> unPinMessageCommand;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Keeps from updating BindableMessages from two places at once
        /// </summary>
        private static SemaphoreSlim SemaphoreSlim { get; } = new SemaphoreSlim(1, 1);

        private bool _AtTop;

        public bool NewItemsLoading
        {
            get => _NewItemsLoading;
            set => Set(ref _NewItemsLoading, value);
        }
        private bool _NewItemsLoading;

        public bool OldItemsLoading
        {
            get => _OldItemsLoading;
            set => Set(ref _OldItemsLoading, value);
        }
        private bool _OldItemsLoading;

        public bool ItemsLoading => _NewItemsLoading || _OldItemsLoading;

        public string MessageText
        {
            get => _MessageText;
            set => Set(ref _MessageText, value);
        }
        private string _MessageText = "";

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

        /// <summary>
        /// Gets the collection of grouped feeds to display
        /// </summary>
        [NotNull]
        public ObservableRangeCollection<BindableMessage> BindableMessages { get; private set; } =
            new ObservableRangeCollection<BindableMessage>();

        #endregion
    }
}
