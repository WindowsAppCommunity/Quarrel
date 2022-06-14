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
        public BindableSelfUser? GetMe()
        {
            SelfUser? user = _quarrelClient.Self.CurrentUser;
            if (user is null) return null;

            return new BindableSelfUser(_messenger, this, _dispatcherService, user);
        }

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
        public BindableUser? GetUser(ulong id)
        {
            var user = _quarrelClient.Users.GetUser(id);
            if (user is null) return null;

            return new BindableUser(_messenger, this, _dispatcherService, user);
        }

        /// <inheritdoc/>
        public BindableGuild[] GetMyGuilds()
        {
            Guild[] rawGuilds = _quarrelClient.Guilds.GetMyGuilds();
            BindableGuild[] guilds = new BindableGuild[rawGuilds.Length];
            for (int i = 0; i < rawGuilds.Length; i++)
            {
                guilds[i] = new BindableGuild(_messenger, this, _dispatcherService, rawGuilds[i]);
            }

            return guilds;
        }

        /// <inheritdoc/>
        public async Task ModifyGuild(ulong id, ModifyGuild modifyGuild)
            => await _quarrelClient.Guilds.ModifyGuild(id, modifyGuild);

        /// <inheritdoc/>
        public BindableGuildFolder[] GetMyGuildFolders()
        {
            GuildFolder[] rawFolders = _quarrelClient.Guilds.GetMyGuildFolders();
            BindableGuildFolder[] folders = new BindableGuildFolder[rawFolders.Length];
            for (int i = 0; i < rawFolders.Length; i++)
            {
                folders[i] = new BindableGuildFolder(_messenger, this, _dispatcherService, rawFolders[i]);
            }

            return folders;
        }

        /// <inheritdoc/>
        public async Task<Message[]> GetChannelMessagesAsync(IBindableMessageChannel channel, ulong? beforeId = null)
        {
            var rawMessages = await _quarrelClient.Messages.GetMessagesAsync(channel.Id, channel.GuildId, beforeId);
            Guard.IsNotNull(rawMessages, nameof(rawMessages));
            return rawMessages;
        }
        
        /// <inheritdoc/>
        public BindableGuildChannel?[] GetGuildChannels(BindableGuild guild, out IBindableSelectableChannel? selectedChannel)
        {
            selectedChannel = null;
            IGuildChannel[] rawChannels = guild.Guild.GetChannels();
            Array.Sort(rawChannels, Comparer<IGuildChannel>.Create((item1, item2) =>
            {
                bool is1Voice = item1.Type is ChannelType.GuildVoice or ChannelType.StageVoice;
                bool is2Voice = item2.Type is ChannelType.GuildVoice or ChannelType.StageVoice;
                if (is1Voice && !is2Voice)
                {
                    return 1;
                }
                 if (is2Voice && !is1Voice)
                {
                    return -1;
                }

                return item1.Position.CompareTo(item2.Position);
            }));

            GuildMember? member = _quarrelClient.Members.GetMyGuildMember(guild.Guild.Id);
            Guard.IsNotNull(member, nameof(member));
            BindableGuildChannel?[] channels = new BindableGuildChannel[rawChannels.Length];
            var categories = new Dictionary<ulong, BindableCategoryChannel>();
            
            // Once for categories
            for (int i = 0; i < rawChannels.Length; i++)
            {
                var channel = rawChannels[i];
                if (channel is CategoryChannel categoryChannel)
                {
                    var bindableCategoryChannel = new BindableCategoryChannel(_messenger, _clipboardService, this, _dispatcherService, categoryChannel, member);
                    categories.Add(channel.Id, bindableCategoryChannel);
                    channels[i] = bindableCategoryChannel;
                }
            }

            for (int i = 0; i < rawChannels.Length; i++)
            {
                ref BindableGuildChannel? channel = ref channels[i];
                if (channel is null && rawChannels[i] is INestedChannel nestedChannel)
                {
                    BindableCategoryChannel? category = null;
                    if (nestedChannel.CategoryId.HasValue)
                    {
                        category = categories[nestedChannel.CategoryId.Value];
                    }

                    channel = BindableGuildChannel.Create(_messenger, _clipboardService, this, _localizationService, _dispatcherService, nestedChannel, member, category);

                    if (channel is not null && (channel.Channel.Id == guild.SelectedChannelId || (selectedChannel is null && channel.IsAccessible)) &&
                        channel is IBindableSelectableChannel messageChannel)
                    {
                        selectedChannel = messageChannel;
                    }
                }
            }

            return channels;
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
                channels[i] = BindablePrivateChannel.Create(_messenger, _clipboardService, this, _localizationService, _dispatcherService, channel);

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
        public BindableGuildMember? GetGuildMember(ulong userId, ulong guildId)
        {
            var member = _quarrelClient.Members.GetGuildMember(userId, guildId);
            if (member is not null)
            {
                return new BindableGuildMember(_messenger, this, _dispatcherService, member);
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task MarkRead(ulong channelId, ulong messageId)
        {
            _analyticsService.Log(LoggedEvent.MarkRead);
            await _quarrelClient.Messages.MarkRead(channelId, messageId);
        }

        /// <inheritdoc/>
        public async Task SendMessage(ulong channelId, string content)
        {
            _analyticsService.Log(LoggedEvent.MessageSent);
            await _quarrelClient.Messages.SendMessage(channelId, content);
        }

        /// <inheritdoc/>
        public async Task DeleteMessage(ulong channelId, ulong messageId)
        {
            _analyticsService.Log(LoggedEvent.MessageDeleted);
            await _quarrelClient.Messages.DeleteMessage(channelId, messageId);
        }

        /// <inheritdoc/>
        public async Task StartCall(ulong channelId)
        {
            _analyticsService.Log(LoggedEvent.StartedCall);
            await _quarrelClient.Channels.StartCall(channelId);
        }

        /// <inheritdoc/>
        public async Task SetStatus(UserStatus status)
        {
            await _quarrelClient.Self.UpdateStatus(status);
        }
    }
}
