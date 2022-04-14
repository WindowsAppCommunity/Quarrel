// Quarrel © 2022

using Discord.API.Gateways.Models;
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
            var frame = new SocketFrame<GuildRequestMembers>()
            {
                Operation = OperationCode.RequestGuildMembers,
                Payload = payload,
            };

            await SendMessageAsync(frame);
        }

        public async Task RequestAllGuildMembers(ulong guildId)
        {
            await RequestGuildMembers(new ulong[] { guildId }, string.Empty);
        }

        public async Task<bool> SubscribeToGuildAsync(ulong[] channelIds)
        {
            // Maximum size 
            if (channelIds.Length > 193)
            {
                return false;
            }

            var frame = new SocketFrame<ulong[]>()
            {
                Operation = OperationCode.SubscribeToGuild,
                Payload = channelIds,
            };

            await SendMessageAsync(frame);
            return true;
        }

        public async Task UpdateStatusAsync(string status, int? idleSince, bool isAfk)
        {
            StatusUpdate payload = new StatusUpdate()
            {
                Status = status,
                IdleSince = idleSince,
                IsAFK = isAfk,
            };

            await UpdateStatusAsync(payload);
        }

        public async Task UpdateStatusAsync(StatusUpdate payload)
        {
            var frame = new SocketFrame<StatusUpdate>()
            {
                Operation = OperationCode.StatusUpdate,
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
            var frame = new SocketFrame<VoiceStatusUpdate>()
            {
                Operation = OperationCode.VoiceStateUpdate,
                Payload = payload,
            };

            await SendMessageAsync(frame);
        }
    }
}
