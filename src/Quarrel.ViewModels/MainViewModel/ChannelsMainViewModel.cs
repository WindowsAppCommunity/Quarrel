// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using DiscordAPI.Models.Messages;
using GalaSoft.MvvmLight.Command;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Models.Bindables.Messages;
using Quarrel.ViewModels.Models.Bindables.Users;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private RelayCommand<BindableChannel> _navigateChannelCommand;
        private RelayCommand _navigateToFriends;
        private BindableChannel _currentChannel;

        /// <summary>
        /// Gets a command that sends Messenger Request to change Channel.
        /// </summary>
        public RelayCommand<BindableChannel> NavigateChannelCommand => _navigateChannelCommand = _navigateChannelCommand ?? new RelayCommand<BindableChannel>(async (channel) =>
        {
            if (channel.IsCategory)
            {
                bool newState = !channel.Collapsed;
                for (int i = CurrentGuild.Channels.IndexOf(channel);
                    i < CurrentGuild.Channels.Count
                    && CurrentGuild.Channels[i] != null
                    && CurrentGuild.Channels[i].ParentId == channel.Model.Id;
                    i++)
                {
                    CurrentGuild.Channels[i].Collapsed = newState;
                }
            }
            else if (channel.IsVoiceChannel)
            {
                if (channel.Model is GuildChannel gChannel)
                {
                    await _gatewayService.Gateway.VoiceStatusUpdate(CurrentGuild.Model.Id, gChannel.Id, false, false);
                }

                _analyticsService.Log(Constants.Analytics.Events.JoinVoiceChannel);
            }
            else if (channel.Permissions.ReadMessages)
            {
                CurrentChannel = channel;

                await Task.Run(() =>
                {
                    MessengerInstance.Send(new ChannelNavigateMessage(channel));

                    _analyticsService.Log(
                        channel.IsPrivateChannel
                            ? Constants.Analytics.Events.OpenDMChannel
                            : Constants.Analytics.Events.OpenGuildChannel,
                        ("channel-id", channel.Model.Id),
                        ("guild-id", channel.GuildId),
                        ("type", channel.Model.Type.ToString()));
                });
            }
        });

        /// <summary>
        /// Gets a command that sets null channel and clear messages to show Friends Panel.
        /// </summary>
        public RelayCommand NavigateToFriends => _navigateToFriends = new RelayCommand(() =>
        {
            if (CurrentChannel != null)
            {
                CurrentChannel.Selected = false;
                CurrentChannel = null;
            }

            BindableMessages.Clear();
        });

        /// <summary>
        /// Gets or sets the currently open channel.
        /// </summary>
        public BindableChannel CurrentChannel
        {
            get => _currentChannel;
            set
            {
                // Deselect
                if (_currentChannel != null)
                {
                    _currentChannel.Selected = false;
                }

                Set(ref _currentChannel, value);

                // Select
                if (_currentChannel != null)
                {
                    _currentChannel.Selected = true;
                }
            }
        }

        /// <summary>
        /// Gets a hashed collection of all channels in loaded by the client, by id.
        /// </summary>
        public IDictionary<string, BindableChannel> AllChannels { get; } = new ConcurrentDictionary<string, BindableChannel>();

        /// <summary>
        /// Gets a hashed collection all channel's settings, by id.
        /// </summary>
        public IDictionary<string, ChannelOverride> ChannelSettings { get; } =
            new ConcurrentDictionary<string, ChannelOverride>();

        private void RegisterChannelsMessages()
        {
            MessengerInstance.Register<GatewayTypingStartedMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (AllChannels.TryGetValue(m.TypingStart.ChannelId, out BindableChannel bChannel))
                    {
                        if (bChannel.Typers.TryRemove(m.TypingStart.UserId, out Timer oldTimer))
                        {
                            oldTimer.Dispose();
                        }

                        Timer timer = new Timer(
                            _ =>
                            {
                                if (bChannel.Typers.TryRemove(m.TypingStart.UserId, out Timer oldUser))
                                {
                                    oldUser.Dispose();
                                }

                                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                                {
                                    bChannel.RaisePropertyChanged(nameof(bChannel.IsTyping));
                                    bChannel.RaisePropertyChanged(nameof(bChannel.TypingText));
                                });
                            },
                            null,
                            8 * 1000,
                            0);

                        bChannel.Typers.TryAdd(m.TypingStart.UserId, timer);

                        _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                        {
                            bChannel.RaisePropertyChanged(nameof(bChannel.IsTyping));
                            bChannel.RaisePropertyChanged(nameof(bChannel.TypingText));
                        });
                    }
                });
            });

            MessengerInstance.Register<ChannelNavigateMessage>(this, async m =>
            {
                await SemaphoreSlim.WaitAsync();
                try
                {
                    _atTop = false;
                    _dispatcherHelper.CheckBeginInvokeOnUi(() => { NewItemsLoading = true; });
                    IList<Message> itemList = null;
                    try
                    {
                        itemList = await _discordService.ChannelService.GetChannelMessages(m.Channel.Model.Id);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        return;
                    }

                    Message lastItem = null;

                    BindableMessage scrollItem = null;

                    List<BindableMessage> messages = new List<BindableMessage>();

                    IReadOnlyDictionary<string, BindableGuildMember> guildMembers = _currentGuild.Model.Id != "DM"
                        ? _guildsService.GetAndRequestGuildMembers(
                            itemList.Select(x => x.User.Id).Distinct(),
                            _currentGuild.Model.Id)
                        : null;

                    int i = itemList.Count;

                    foreach (Message item in itemList.Reverse())
                    {
                        messages.Add(new BindableMessage(
                            item,
                            lastItem != null && lastItem.User.Id == item.User.Id,
                            lastItem != null && m.Channel.ReadState != null && lastItem.Id == m.Channel.ReadState.LastMessageId,
                            guildMembers != null && guildMembers.TryGetValue(item.User.Id, out BindableGuildMember member) ? member : null));

                        if (lastItem != null && m.Channel.ReadState != null &&
                            lastItem.Id == m.Channel.ReadState.LastMessageId)
                        {
                            scrollItem = messages.LastOrDefault();
                        }

                        lastItem = item;

                        i--;
                    }

                    _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        BindableMessages.Clear();
                        BindableMessages.AddRange(messages);
                        ScrollTo?.Invoke(this, scrollItem ?? BindableMessages.LastOrDefault());
                        NewItemsLoading = false;
                    });
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
            });

            MessengerInstance.Register<GatewayMessageAckMessage>(this, m =>
            {
                var channel = _channelsService.GetChannel(m.ChannelId);
                channel?.UpdateLRMID(m.MessageId);
            });
        }
    }
}
