// Quarrel © 2022

using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Channels.Abstract
{
    /// <summary>
    /// A wrapper of an <see cref="IPrivateChannel"/> that can be bound to the UI.
    /// </summary>
    public abstract class BindablePrivateChannel : BindableChannel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindablePrivateChannel"/> class.
        /// </summary>
        internal BindablePrivateChannel(IDiscordService discordService, IDispatcherService dispatcherService, PrivateChannel privateChannel) :
            base(discordService, dispatcherService, privateChannel)
        {
        }

        public static BindablePrivateChannel? Create(IDiscordService discordService, IDispatcherService dispatcherService, IPrivateChannel channel)
        {
            return BindableChannel.Create(discordService, dispatcherService, channel) as BindablePrivateChannel;
        }
    }
}
