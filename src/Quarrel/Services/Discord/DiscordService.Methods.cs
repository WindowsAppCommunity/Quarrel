// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Enums.Channels;
using Discord.API.Models.Enums.Users;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Guilds;
using Quarrel.Bindables.Users;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Guilds;
using Quarrel.Client.Models.Messages;
using Quarrel.Client.Models.Settings;
using Quarrel.Client.Models.Users;
using Quarrel.Services.Analytics.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
        /// <inheritdoc/>
        public ulong? MyId => _quarrelClient.Self.CurrentUser?.Id;
        
        /// <inheritdoc/>
        public async Task ModifyMe(ModifySelfUser modifyUser)
            => await _quarrelClient.Self.ModifyMe(modifyUser);

        /// <inheritdoc/>
        public UserSettings? GetSettings() 
            => _quarrelClient.Self.Settings;

        /// <inheritdoc/>
        public async Task ModifySettings(ModifyUserSettings modifySettings)
            => await _quarrelClient.Self.ModifySettings(modifySettings);

        /// <inheritdoc/>
        public async Task ModifyGuild(ulong id, ModifyGuild modifyGuild)
            => await _quarrelClient.Guilds.ModifyGuild(id, modifyGuild);

        /// <inheritdoc/>
        public async Task<Message[]> GetChannelMessagesAsync(IBindableMessageChannel channel, ulong? beforeId = null)
        {
            var rawMessages = await _quarrelClient.Messages.GetMessagesAsync(channel.Id, channel.GuildId, beforeId);
            Guard.IsNotNull(rawMessages, nameof(rawMessages));
            return rawMessages;
        }
    
        /// <inheritdoc/>
        public BindablePrivateChannel?[] GetPrivateChannels(BindableHomeItem home, out IBindableSelectableChannel? selectedChannel)
        {
            selectedChannel = null;
            IPrivateChannel[] rawChannels = _quarrelClient.Channels.GetPrivateChannels();
            BindablePrivateChannel?[] channels = new BindablePrivateChannel[rawChannels.Length];
            int i = 0;
            foreach (var channel in rawChannels)
            {
                channels[i] = BindablePrivateChannel.Create(_messenger, _clipboardService, this, _quarrelClient, _localizationService, _dispatcherService, channel);

                if (channels[i] is IBindableSelectableChannel selectableChannel &&
                    selectableChannel.Id == home.SelectedChannelId)
                {
                    selectedChannel = selectableChannel;
                }

                i++;
            }

            return channels;
        }
        
        /// <inheritdoc/>
        public async Task MarkRead(ulong channelId, ulong messageId)
        {
            _loggingService.Log(LoggedEvent.MarkRead);
            await _quarrelClient.Messages.MarkRead(channelId, messageId);
        }

        /// <inheritdoc/>
        public async Task SendMessage(ulong channelId, string content)
        {
            _loggingService.Log(LoggedEvent.MessageSent);
            await _quarrelClient.Messages.SendMessage(channelId, content);
        }

        /// <inheritdoc/>
        public async Task DeleteMessage(ulong channelId, ulong messageId)
        {
            _loggingService.Log(LoggedEvent.MessageDeleted);
            await _quarrelClient.Messages.DeleteMessage(channelId, messageId);
        }

        /// <inheritdoc/>
        public async Task StartCall(ulong channelId)
        {
            _loggingService.Log(LoggedEvent.StartedCall);
            await _quarrelClient.Channels.StartCall(channelId);
        }

        /// <inheritdoc/>
        public async Task JoinCall(ulong channelId, ulong? guildId)
        {
            _loggingService.Log(LoggedEvent.JoinedCall,
                ("Private Call", $"{guildId is null}"));
            await _quarrelClient.Channels.JoinCall(channelId, guildId);
        }

        /// <inheritdoc/>
        public async Task LeaveCall()
        {
            _loggingService.Log(LoggedEvent.LeftCall);
            await _quarrelClient.Channels.LeaveCall();
        }
        
        /// <inheritdoc/>
        public async Task JoinStream(ulong userId)
        {
            // TODO: Log event
            await _quarrelClient.Channels.JoinStream(userId);
        }

        /// <inheritdoc/>
        public async Task SetStatus(UserStatus status)
        {
            await _quarrelClient.Self.UpdateStatus(status);
        }
    }
}
