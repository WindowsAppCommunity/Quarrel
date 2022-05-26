// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Enums.Users;
using Discord.API.Models.Json.Messages;
using Discord.API.Models.Json.Settings;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Guilds;
using Quarrel.Client.Models.Messages;
using Quarrel.Client.Models.Settings;
using Quarrel.Client.Models.Users;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.Client
{
    public partial class QuarrelClient
    {
        /// <summary>
        /// Gets the current user for the <see cref="QuarrelClient"/> instance.
        /// </summary>
        public SelfUser? GetMe()
        {
            return CurrentUser;
        }

        /// <summary>
        /// Gets a user by id.
        /// </summary>
        /// <param name="id">The id of the user to get.</param>
        public User? GetUser(ulong id)
        {
            _userMap.TryGetValue(id, out var user);
            return user;
        }

        /// <summary>
        /// Gets the client settings.
        /// </summary>
        public UserSettings? GetSettings()
        {
            return _settings;
        }

        /// <summary>
        /// Modifies user settings.
        /// </summary>
        /// <param name="modifySettings">The settings adjustments.</param>
        public async Task ModifySettings(ModifyUserSettings modifySettings)
        {
            Guard.IsNotNull(_userService, nameof(_userService));

            await _userService.UpdateSettings(modifySettings.ToJsonModel());
        }

        /// <summary>
        /// Gets messages in a channel.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="guildId">The id of the guild the channel belongs to.</param>
        /// <param name="beforeId">The message to get the messages from before.</param>
        public async Task<Message[]> GetMessagesAsync(ulong channelId, ulong? guildId = null, ulong? beforeId = null)
        {
            Guard.IsNotNull(_channelService, nameof(_channelService));

            Func<Task<JsonMessage[]>> request = () => _channelService.GetChannelMessages(channelId);
            if (beforeId.HasValue)
            {
                request = () => _channelService.GetChannelMessagesBefore(channelId, beforeId.Value);
            }

            JsonMessage[]? jsonMessages = await MakeRefitRequest(request);
            Guard.IsNotNull(jsonMessages, nameof(jsonMessages));

            Message[] messages = new Message[jsonMessages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                jsonMessages[i].GuildId = jsonMessages[i].GuildId ?? guildId;
                messages[i] = new Message(jsonMessages[i], this);
            }

            return messages;
        }

        /// <summary>
        /// Gets the current user as a guild member in a specific guild.
        /// </summary>
        /// <param name="guildId">The id of the guild to get the guild member for.</param>
        public GuildMember? GetMyGuildMember(ulong guildId)
        {
            Guard.IsNotNull(_selfUser, nameof(_selfUser));
            return GetGuildMember(guildId, _selfUser.Id);
        }

        /// <summary>
        /// Gets a guild member by guild and user id.
        /// </summary>
        /// <param name="guildId">The id for the guild of the guild member.</param>
        /// <param name="userId">The id for the user in the guild.</param>
        /// <returns></returns>
        public GuildMember? GetGuildMember(ulong guildId, ulong userId)
        {
            if (_guildsMemberMap.TryGetValue((guildId, userId), out var member))
            {
                return member;
            }

            return null;
        }

        /// <summary>
        /// Gets the user's guild according to their order in settings.
        /// </summary>
        public Guild[] GetMyGuilds()
        {
            Guard.IsNotNull(_settings, nameof(_settings));

            ulong[] order = _settings.GuildOrder;
            Guild[] guildArray = new Guild[order.Length];

            int realCount = 0;
            for (int i = 0; i < order.Length; i++)
            {
                Guild? guild = GetGuildInternal(order[realCount]);
                if (guild != null)
                {
                    guildArray[i] = guild;
                    realCount++;
                }
            }

            Array.Resize(ref guildArray, realCount);

            return guildArray;
        }

        /// <summary>
        /// Modifes a guild.
        /// </summary>
        /// <param name="id">The id of the guild.</param>
        /// <param name="modifyGuild"></param>
        /// <returns></returns>
        public async Task ModifyGuild(ulong id, ModifyGuild modifyGuild)
        {
            Guard.IsNotNull(_guildService, nameof(_guildService));

            await _guildService.ModifyGuild(id, modifyGuild.ToJsonModel());
        }

        /// <summary>
        /// Gets the guild folders for the current user.
        /// </summary>
        /// <remarks>
        /// Folders with null id should be handled as if they didn't exist and their children are actually in the root list.
        /// </remarks>
        public GuildFolder[] GetMyGuildFolders()
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            return _settings.Folders;
        }

        public IPrivateChannel[] GetPrivateChannels()
        {
            IPrivateChannel[] privateChannels = new IPrivateChannel[_privateChannels.Count];
            int i = 0;
            foreach (var channelId in _privateChannels)
            {
                var channel = GetChannelInternal(channelId);
                if (channel is IPrivateChannel directChannel)
                {
                    privateChannels[i] = directChannel;
                    i++;
                }
            }

            // Nullability is improperly accessed here
#pragma warning disable CS8629
            Array.Resize(ref privateChannels, i);
            Array.Sort(privateChannels, Comparer<IPrivateChannel>.Create((item1, item2) =>
            {
                bool i1Null = !item1.LastMessageId.HasValue;
                bool i2Null = !item2.LastMessageId.HasValue;

                if (i1Null && i2Null) return 0;
                if (i2Null) return -1;
                if (i1Null) return 1;

                long compare = (long)item2.LastMessageId.Value - (long)item1.LastMessageId.Value;
                if (compare < 0) return -1;
                if (compare > 0) return 1;
                return 0;
            }));
#pragma warning restore CS8629

            return privateChannels;
        }

        /// <summary>
        /// Marks a message as the last read message in the channel.
        /// </summary>
        /// <param name="channelId">The id of the channel.</param>
        /// <param name="messageId">The id of the message.</param>
        /// <returns></returns>
        public async Task MarkRead(ulong channelId, ulong messageId)
        {
            Guard.IsNotNull(_channelService, nameof(_channelService));
            await MakeRefitRequest(() => _channelService.MarkRead(channelId, messageId));
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="channelId">The channel to send the message in.</param>
        /// <param name="content">The content of the message.</param>
        public async Task SendMessage(ulong channelId, string content)
        {
            Guard.IsNotNull(_channelService, nameof(_channelService));
            ulong nonce = (ulong)new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() << 22;
            JsonMessageUpsert message = new JsonMessageUpsert(content, false, $"{nonce}");
            await MakeRefitRequest(() => _channelService.CreateMessage(channelId, message));
        }

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="channelId">The id of channel the message belongs to.</param>
        /// <param name="messageId">The id of the message to delete.</param>
        public async Task DeleteMessage(ulong channelId, ulong messageId)
        {
            Guard.IsNotNull(_channelService, nameof(_channelService));
            await MakeRefitRequest(() => _channelService.DeleteMessage(channelId, messageId));
        }

        /// <summary>
        /// Updates the user's online status.
        /// </summary>
        /// <param name="status">The new online status to set.</param>
        public async Task UpdateStatus(UserStatus status)
        {
            Guard.IsNotNull(_gateway, nameof(_gateway));
            Guard.IsNotNull(_userService, nameof(_userService));

            await _gateway.UpdateStatusAsync(status);
            var settingsUpdate = new JsonModifyUserSettings()
            {
                Status = status.GetStringValue(),
            };
            await _userService.UpdateSettings(settingsUpdate);
        }

        private async Task MakeRefitRequest(Func<Task> request)
        {
            try
            {
                await request();
            }
            catch (ApiException ex)
            {
                HttpExceptionHandled?.Invoke(this, ex);
            }
        }

        private async Task<T?> MakeRefitRequest<T>(Func<Task<T>> request)
        {
            try
            {
                return await request();
            }
            catch (ApiException ex)
            {
                HttpExceptionHandled?.Invoke(this, ex);
                return default;
            }
            catch { return default; }
        }
    }
}
