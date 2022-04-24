// Quarrel © 2022

using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of an <see cref="IDirectChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableDirectChannel : BindablePrivateChannel
    {
        internal BindableDirectChannel(IDiscordService discordService, IDispatcherService dispatcherService, DirectChannel directChannel) :
            base(discordService, dispatcherService, directChannel)
        {
        }
    }
}
