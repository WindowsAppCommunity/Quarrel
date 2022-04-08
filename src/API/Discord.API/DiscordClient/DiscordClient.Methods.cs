// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Guilds;
using Discord.API.Models.Messages;
using Discord.API.Models.Settings;
using Discord.API.Models.Users;
using System;
using System.Threading.Tasks;

namespace Discord.API
{
    public partial class DiscordClient
    {
        public SelfUser? GetMe()
        {
            return _selfUser;
        }

        public async Task<Message[]> GetMessagesAsync(ulong channelId)
        {
            Guard.IsNotNull(_channelService, nameof(_channelService));

            var jsonMessages = await _channelService.GetChannelMessages(channelId);

            Message[] messages = new Message[jsonMessages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = new Message(jsonMessages[i], this);
            }

            return messages;
        }

        public GuildMember? GetMyGuildMember(ulong guildId)
        {
            Guard.IsNotNull(_selfUser, nameof(_selfUser));
            return GetGuildMember(guildId, _selfUser.Id);
        }

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

        public GuildFolder[] GetMyGuildFolders()
        {
            return _settings.Folders;
        }
    }
}
