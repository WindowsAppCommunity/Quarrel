// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
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

        /// <summary>
        /// Gets a command that marks the channel as read.
        /// </summary>
        RelayCommand MarkAsReadCommand { get; }

        /// <summary>
        /// Marks the last message in the channel as read.
        /// </summary>
        void MarkRead();
    }
}
