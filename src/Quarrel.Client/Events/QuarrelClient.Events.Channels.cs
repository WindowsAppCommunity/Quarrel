// Quarrel © 2022

using Discord.API.Models.Json.Channels;
using Quarrel.Client.Models.Channels.Abstract;

namespace Quarrel.Client
{
    /// <inheritdoc/>
    public partial class QuarrelClient
    {
        private void OnChannelUpdated(JsonChannel jsonChannel)
        {
            if (_channelMap.TryGetValue(jsonChannel.Id, out Channel channel))
            {
                channel.UpdateFromJsonChannel(jsonChannel);

                ChannelUpdated?.Invoke(this, channel);
            }
        }
    }
}
