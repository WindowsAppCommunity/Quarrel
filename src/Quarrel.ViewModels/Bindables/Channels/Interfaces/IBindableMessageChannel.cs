// Quarrel © 2022

using Quarrel.Client.Models.Channels.Interfaces;

namespace Quarrel.Bindables.Channels.Interfaces
{
    /// <summary>
    /// An interface for wrapping a <see cref="IMessageChannel"/> in a bindable context.
    /// </summary>
    public interface IBindableMessageChannel : IBindableSelectableChannel
    {
        /// <summary>
        /// Gets the wrapped <see cref="IMessageChannel"/>.
        /// </summary>
        IMessageChannel MessageChannel { get; }
    }
}
