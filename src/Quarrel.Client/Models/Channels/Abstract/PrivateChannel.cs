// Quarrel © 2022

using Discord.API.Models.Json.Channels;

namespace Quarrel.Client.Models.Channels.Abstract
{
    public abstract class PrivateChannel : Channel
    {
        internal PrivateChannel(JsonChannel restChannel, QuarrelClient context) :
            base(restChannel, context)
        {
        }
    }
}
