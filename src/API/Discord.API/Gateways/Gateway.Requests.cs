﻿// Quarrel © 2022

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
            var frame = new GatewaySocketFrame<GuildRequestMembers>()
            {
                Operation = GatewayOperation.RequestGuildMembers,
                Payload = payload,
            };

            await SendMessageAsync(frame);
        }

        public async Task RequestAllGuildMembers(ulong guildId)
        {
            await RequestGuildMembers(new ulong[] { guildId }, string.Empty);
        }

        public void SubscribeToGuildAsync(ulong[] channelIds)
        {
            throw new NotImplementedException();
        }

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
            var frame = new GatewaySocketFrame<StatusUpdate>()
            {
                Operation = GatewayOperation.PresenceUpdate,
                Payload = payload,
            };

            await SendMessageAsync(frame);
        }

        public async Task VoiceStatusUpdateAsync(ulong guildId, ulong channelId, bool selfMute, bool selfDeaf)
        {
            var payload = new VoiceStatusUpdate()
            {
                GuildId = guildId,
                ChannelId = channelId,
                Deaf = selfDeaf,
                Mute = selfMute
            };

            await VoiceStatusUpdateAsync(payload);
        }

        public async Task VoiceStatusUpdateAsync(VoiceStatusUpdate payload)
        {
            var frame = new GatewaySocketFrame<VoiceStatusUpdate>()
            {
                Operation = GatewayOperation.VoiceStateUpdate,
                Payload = payload,
            };

            await SendMessageAsync(frame);
        }
    }
}
