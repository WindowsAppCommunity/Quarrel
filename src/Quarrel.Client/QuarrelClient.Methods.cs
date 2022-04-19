﻿// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Messages;
using Quarrel.Client.Models.Guilds;
using Quarrel.Client.Models.Messages;
using Quarrel.Client.Models.Settings;
using Quarrel.Client.Models.Users;
using Refit;
using System;
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
        /// Gets messages in a channel.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        public async Task<Message[]> GetMessagesAsync(ulong channelId)
        {
            Guard.IsNotNull(_channelService, nameof(_channelService));

            JsonMessage[]? jsonMessages = await MakeRefitRequest(() => _channelService.GetChannelMessages(channelId));
            Guard.IsNotNull(jsonMessages, nameof(jsonMessages));

            Message[] messages = new Message[jsonMessages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
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