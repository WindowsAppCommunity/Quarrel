using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Quarrel.ViewModels
{
    public partial class MainViewModel
    {
        #region Commands

        #region Navigation

        /// <summary>
        /// Sends Messenger Request to change Channel
        /// </summary>
        private RelayCommand<BindableChannel> navigateChannelCommand;
        public RelayCommand<BindableChannel> NavigateChannelCommand => navigateChannelCommand ??=
            new RelayCommand<BindableChannel>(async (channel) =>
            {
                if (channel.IsCategory)
                {
                    bool newState = !channel.Collapsed;
                    for (int i = CurrentGuild.Channels.IndexOf(channel);
                        i < CurrentGuild.Channels.Count
                        && CurrentGuild.Channels[i] != null
                        && CurrentGuild.Channels[i].ParentId == channel.Model.Id;
                        i++)
                        CurrentGuild.Channels[i].Collapsed = newState;
                }
                else if (channel.IsVoiceChannel)
                {
                    if (channel.Model is GuildChannel gChannel)
                        await DiscordService.Gateway.Gateway.VoiceStatusUpdate(CurrentGuild.Model.Id, gChannel.Id, false,
                            false);
                }
                else if (channel.Permissions.ReadMessages)
                {
                    CurrentChannel = channel;
                    MessengerInstance.Send(new ChannelNavigateMessage(channel, CurrentGuild));
                }
            });

        /// <summary>
        /// Sets null channel and clear messages to show Friends Panel
        /// </summary>
        private RelayCommand navigateToFriends;
        public RelayCommand NavigateToFriends => navigateToFriends = new RelayCommand(() =>
        {
            if (CurrentChannel != null)
            {
                CurrentChannel.Selected = false;
                CurrentChannel = null;
            }

            BindableMessages.Clear();
        });

        #endregion

        #endregion

        #region Methods

        private void RegisterChannelsMessages()
        {
            #region Gateway

            MessengerInstance.Register<GatewayTypingStartedMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (ChannelsService.AllChannels.TryGetValue(m.TypingStart.ChannelId, out BindableChannel bChannel))
                    {
                        if (bChannel.Typers.TryRemove(m.TypingStart.UserId, out Timer oldTimer)) oldTimer.Dispose();

                        Timer timer = new Timer(_ =>
                        {
                            if (bChannel.Typers.TryRemove(m.TypingStart.UserId, out Timer oldUser))
                                oldUser.Dispose();

                            DispatcherHelper.CheckBeginInvokeOnUi(() =>
                            {
                                bChannel.RaisePropertyChanged(nameof(bChannel.IsTyping));
                                bChannel.RaisePropertyChanged(nameof(bChannel.TypingText));
                            });
                        }, null, 8 * 1000, 0);

                        bChannel.Typers.TryAdd(m.TypingStart.UserId, timer);

                        DispatcherHelper.CheckBeginInvokeOnUi(() =>
                        {
                            bChannel.RaisePropertyChanged(nameof(bChannel.IsTyping));
                            bChannel.RaisePropertyChanged(nameof(bChannel.TypingText));
                        });
                    }
                });
            });

            #endregion

            #region Navigation

            MessengerInstance.Register<ChannelNavigateMessage>(this, async m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() => { CurrentChannel = m.Channel; });

                await SemaphoreSlim.WaitAsync();
                try
                {
                    _AtTop = false;
                    NewItemsLoading = true;
                    IList<Message> itemList = null;
                    try
                    {
                        itemList = await DiscordService.ChannelService.GetChannelMessages(m.Channel.Model.Id);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        return;
                    }

                    Message lastItem = null;

                    BindableMessage scrollItem = null;

                    List<BindableMessage> messages = new List<BindableMessage>();


                    IReadOnlyDictionary<string, BindableGuildMember> guildMembers = guildId != "DM"
                        ? GuildsService.GetAndRequestGuildMembers(itemList.Select(x => x.User.Id).Distinct(),
                            guildId)
                        : null;

                    int i = itemList.Count;

                    foreach (Message item in itemList.Reverse())
                    {
                        messages.Add(new BindableMessage(item, guildId,
                            lastItem != null && lastItem.User.Id == item.User.Id,
                            lastItem != null && m.Channel.ReadState != null &&
                            lastItem.Id == m.Channel.ReadState.LastMessageId,
                            guildMembers != null && guildMembers.TryGetValue(item.User.Id, out BindableGuildMember member)
                                ? member
                                : null));

                        if (lastItem != null && m.Channel.ReadState != null &&
                            lastItem.Id == m.Channel.ReadState.LastMessageId) scrollItem = messages.LastOrDefault();

                        lastItem = item;


                        if (!SettingsService.Roaming.GetValue<bool>(SettingKeys.AdsRemoved) && i % 10 == 0)
                        {
                            messages.Add(new BindableMessage(new Message() { Id = "Ad", ChannelId = CurrentChannel.Model.Id },
                                null));
                            lastItem = null;
                        }

                        i--;
                    }

                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        BindableMessages.Clear();
                        BindableMessages.AddRange(messages);
                        ScrollTo?.Invoke(this, scrollItem ?? BindableMessages.LastOrDefault());
                    });
                    NewItemsLoading = false;
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
            });

            #endregion
        }

        #endregion

        #region Properties

        public BindableChannel CurrentChannel
        {
            get => _CurrentChannel;
            set => Set(ref _CurrentChannel, value);
        }
        private BindableChannel _CurrentChannel;

        #endregion
    }
}
