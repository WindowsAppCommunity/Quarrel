// Quarrel © 2022

using Discord.API.Gateways.Models.Messages;

namespace Quarrel.Client.Models.Messages
{
    public class AllReactionsRemoved
    {
        internal AllReactionsRemoved(JsonReactionRemoveAll jsonReactionRemoveAll)
        {
            ChannelId = jsonReactionRemoveAll.ChannelId;
            MessageId = jsonReactionRemoveAll.MessageId;
        }

        public ulong ChannelId { get; }

        public ulong MessageId { get; }
    }
}
