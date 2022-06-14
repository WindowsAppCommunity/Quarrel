// Quarrel © 2022

using Discord.API.Gateways.Models;
using Discord.API.Models.Enums.Users;
using System;
using System.Threading.Tasks;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        public async Task RequestGuildMembers(ulong[] guildIds, string query, int limit = 0, bool? presences = null, ulong[]? userIds = null)
        {
            var payload = new GuildRequestMembers()
            {
                GuildIds = guildIds,
                Query = query,
                Limit = limit,
                Presences = presences,
                UserIds = userIds,
            };

            await RequestGuildMembers(payload);
        }

        public async Task RequestGuildMembers(GuildRequestMembers payload)
        {
            await SendMessageAsync(GatewayOperation.RequestGuildMembers, payload);
        }

        public async Task RequestAllGuildMembers(ulong guildId)
        {
            await RequestGuildMembers(new ulong[] { guildId }, string.Empty);
        }

        //[Obsolete("Guild subscription is deprecated in favor of lazy guilds.")]
        //public void SubscribeToGuildAsync(ulong[] channelIds)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task UpdateStatusAsync(UserStatus status, int? idleSince = null, bool isAfk = false)
        {
            var payload = new StatusUpdate()
            {
                Status = status.GetStringValue(),
                IdleSince = idleSince,
                IsAFK = isAfk,
            };

            await UpdateStatusAsync(payload);
        }

        private async Task UpdateStatusAsync(StatusUpdate payload)
        {
            await SendMessageAsync(GatewayOperation.PresenceUpdate, payload);
        }

        public async Task VoiceStatusUpdateAsync(ulong channelId, ulong? guildId = null, bool selfMute = false, bool selfDeaf = false)
        {
            var payload = new VoiceStatusUpdate()
            {
                GuildId = guildId,
                ChannelId = channelId,
                Deaf = selfDeaf,
                Mute = selfMute,
            };

            await VoiceStatusUpdateAsync(payload);
        }

        public async Task VoiceStatusUpdateAsync(VoiceStatusUpdate payload)
        {
            await SendMessageAsync(GatewayOperation.VoiceStateUpdate, payload);
        }
    }
}
