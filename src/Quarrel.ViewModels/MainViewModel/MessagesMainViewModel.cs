// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The ViewModel for all data throughout the app.
    /// </summary>
    public partial class MainViewModel
    {
        private RelayCommand<BindableMessage> deleteMessageCommand;
        private RelayCommand<BindableMessage> pinMessageCommand;
        private RelayCommand<BindableMessage> unPinMessageCommand;
        private RelayCommand<BindableMessage> copyMessageIdCommand;
        private bool _atTop;
        private bool _newItemsLoading;
        private bool _oldItemsLoading;

        /// <summary>
        /// Scrolls message list to BindableMessage
        /// </summary>
        public event EventHandler<BindableMessage> ScrollTo;

        /// <summary>
        /// Gets a command that sends API request to delete a message.
        /// </summary>
        public RelayCommand<BindableMessage> DeleteMessageCommand => deleteMessageCommand = deleteMessageCommand ?? new RelayCommand<BindableMessage>(async (message) =>
        {
            await DiscordService.ChannelService.DeleteMessage(message.Model.ChannelId, message.Model.Id);
        });

        /// <summary>
        /// Gets a command that sends API request to pin a message.
        /// </summary>
        public RelayCommand<BindableMessage> PinMessageCommand => pinMessageCommand = pinMessageCommand ?? new RelayCommand<BindableMessage>(async (message) =>
        {
            await DiscordService.ChannelService.AddPinnedChannelMessage(
                message.Model.ChannelId,
                message.Model.Id);
        });

        /// <summary>
        /// Gets a command that sends API request to unpin a message.
        /// </summary>
        public RelayCommand<BindableMessage> UnPinMessageCommand => unPinMessageCommand = unPinMessageCommand ?? new RelayCommand<BindableMessage>(async (message) =>
        {
            await DiscordService.ChannelService.DeletePinnedChannelMessage(
                message.Model.ChannelId,
                message.Model.Id);
        });

        /// <summary>
        /// Gets a command that copies message id to the clipboard.
        /// </summary>
        public RelayCommand<BindableMessage> CopyMessageIdCommand => copyMessageIdCommand = copyMessageIdCommand ?? new RelayCommand<BindableMessage>((message) =>
        {
            ClipboardService.CopyToClipboard(message.Model.Id);
        });

        /// <summary>
        /// Gets the collection of grouped feeds to display.
        /// </summary>
        [NotNull]
        public ObservableRangeCollection<BindableMessage> BindableMessages { get; private set; } =
            new ObservableRangeCollection<BindableMessage>();

        /// <summary>
        /// Gets a value indicating whether or not there are new items loading.
        /// </summary>
        public bool NewItemsLoading
        {
            get => _newItemsLoading;
            private set => Set(ref _newItemsLoading, value);
        }

        /// <summary>
        /// Gets a value indicating whether or not there are older items loading.
        /// </summary>
        public bool OldItemsLoading
        {
            get => _oldItemsLoading;
            private set => Set(ref _oldItemsLoading, value);
        }

        /// <summary>
        /// Gets a value indicating whether or not there are items loading.
        /// </summary>
        public bool ItemsLoading => _newItemsLoading || _oldItemsLoading;

        /// <summary>
        /// Gets a semaphore to keep from updating BindableMessages from two places at once.
        /// </summary>
        private static SemaphoreSlim SemaphoreSlim { get; } = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Loads messages from before the first message in the message list.
        /// </summary>
        public async void LoadOlderMessages()
        {
            if (ItemsLoading || _atTop)
            {
                return;
            }

            await SemaphoreSlim.WaitAsync();
            try
            {
                OldItemsLoading = true;
                IEnumerable<Message> itemList =
                    await DiscordService.ChannelService.GetChannelMessagesBefore(
                        CurrentChannel.Model.Id,
                        BindableMessages.FirstOrDefault().Model.Id);

                List<BindableMessage> messages = new List<BindableMessage>();
                Message lastItem = null;

                if (itemList.Count() < 50)
                {
                    _atTop = true;
                    if (!itemList.Any())
                    {
                        OldItemsLoading = false;
                        return;
                    }
                }

                IReadOnlyDictionary<string, BindableGuildMember> guildMembers = _currentGuild.Model.Id != "DM"
                    ? GuildsService.GetAndRequestGuildMembers(itemList.Select(x => x.User.Id).Distinct(), _currentGuild.Model.Id) : null;

                for (int i = itemList.Count() - 1; i >= 0; i--)
                {
                    Message item = itemList.ElementAt(i);

                    // Can't be last read item
                    messages.Add(new BindableMessage(
                        item,
                        _currentGuild.Model.Id,
                        lastItem != null && lastItem.User.Id == item.User.Id,
                        false,
                        guildMembers == null || !guildMembers.TryGetValue(item.User.Id, out BindableGuildMember member) ? null : member));
                    lastItem = item;
                }

                if (messages.Count > 0)
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        BindableMessages.InsertRange(0, messages, NotifyCollectionChangedAction.Reset);
                        OldItemsLoading = false;
                    });
                }
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Loads messages from after the last message in the message list.
        /// </summary>
        public async void LoadNewerMessages()
        {
            if (ItemsLoading)
            {
                return;
            }

            await SemaphoreSlim.WaitAsync();
            try
            {
                NewItemsLoading = true;
                if (CurrentChannel.Model.LastMessageId != BindableMessages.LastOrDefault().Model.Id)
                {
                    IEnumerable<Message> itemList = null;
                    await Task.Run(async () =>
                        itemList = await DiscordService.ChannelService.GetChannelMessagesAfter(
                            CurrentChannel.Model.Id,
                            BindableMessages.LastOrDefault().Model.Id));

                    List<BindableMessage> messages = new List<BindableMessage>();
                    Message lastItem = null;

                    for (int i = 0; i < itemList.Count(); i++)
                    {
                        Message item = itemList.ElementAt(i);

                        // Can't be last read item
                        messages.Add(new BindableMessage(item, _currentGuild.Model.Id));
                        lastItem = item;
                    }

                    if (messages.Count > 0)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUi(() =>
                        {
                            BindableMessages.AddRange(messages, NotifyCollectionChangedAction.Reset);
                        });
                    }
                }
                else if (CurrentChannel.ReadState == null || CurrentChannel.Model.LastMessageId != CurrentChannel.ReadState.LastMessageId)
                {
                    await DiscordService.ChannelService.AckMessage(
                        CurrentChannel.Model.Id,
                        BindableMessages.LastOrDefault().Model.Id);
                }

                NewItemsLoading = false;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Scrolls to the last message sent by the current user and enters edit mode.
        /// </summary>
        public void ScrollToAndEditLast()
        {
            var userLastMessage = BindableMessages.LastOrDefault(x => x.Model.User.Id == CurrentUserService.CurrentUser.Model.Id);
            if (userLastMessage != null)
            {
                userLastMessage.IsEditing = true;
                ScrollTo?.Invoke(this, userLastMessage);
            }
        }

        private void RegisterMessagesMessages()
        {
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
                    {
                        DispatcherHelper.CheckBeginInvokeOnUi(() =>
                        {
                            channel.ReadState.MentionCount++;
                            if (channel.IsDirectChannel || channel.IsGroupChannel)
                            {
                                int oldIndex = GuildsService.AllGuilds["DM"].Channels.IndexOf(channel);
                                if (oldIndex >= 0)
                                {
                                    GuildsService.AllGuilds["DM"].Channels.Move(oldIndex, 0);
                                }
                            }
                        });
                    }

                    if (CurrentChannel != null && CurrentChannel.Model.Id == channel.Model.Id)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUi(() =>
                        {
                            // Removes typer from Channel if responsible for sending this message
                            channel.Typers.TryRemove(m.Message.User.Id, out _);
                            m.Message.Member.User = m.Message.User;
                            BindableMessages.Add(new BindableMessage(
                                m.Message,
                                channel.Guild.Model.Id ?? "DM",
                                BindableMessages.LastOrDefault().Model.User != null && BindableMessages.LastOrDefault().Model.User.Id == m.Message.User.Id,
                                false,
                                new BindableGuildMember(m.Message.Member, m.Message.GuildId)));
                        });
                    }
                }
            });

            // Handles message deletion
            MessengerInstance.Register<GatewayMessageDeletedMessage>(this, m =>
            {
                if (CurrentChannel != null && CurrentChannel.Model.Id == m.ChannelId)
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        BindableMessage msg = BindableMessages.LastOrDefault(x => x.Model.Id == m.MessageId);
                        if (msg != null)
                        {
                            int index = BindableMessages.IndexOf(msg);
                            if (!msg.IsContinuation && index != BindableMessages.Count - 1)
                            {
                                BindableMessages[index + 1].IsContinuation = false;
                            }

                            BindableMessages.Remove(msg);
                        }
                    });
                }
            });

            // Handles message updated
            MessengerInstance.Register<GatewayMessageUpdatedMessage>(this, m =>
            {
                if (CurrentChannel != null && CurrentChannel.Model.Id == m.Message.ChannelId)
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        BindableMessage msg = BindableMessages.LastOrDefault();
                        msg?.Update(m.Message);
                    });
                }
            });

            MessengerInstance.Register<GatewayReactionAddedMessage>(this, m =>
            {
                if (CurrentChannel != null && CurrentChannel.Model.Id != m.ChannelId)
                {
                    return;
                }

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
        }
    }
}
