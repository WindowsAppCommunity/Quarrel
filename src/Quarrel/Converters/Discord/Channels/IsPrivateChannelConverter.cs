// Quarrel © 2022

using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;

namespace Quarrel.Converters.Discord.Channels
{
    public class IsPrivateChannelConverter
    {
        public static bool Convert(IBindableChannel channel)
        {
            return channel is BindablePrivateChannel;
        }
    }
}
