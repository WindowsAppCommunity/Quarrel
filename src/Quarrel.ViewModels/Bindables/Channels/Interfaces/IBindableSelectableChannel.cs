// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.Bindables.Interfaces;
using Quarrel.Client.Models.Channels.Interfaces;

namespace Quarrel.Bindables.Channels.Interfaces
{
    /// <summary>
    /// An interface for wrapping an <see cref="IChannel"/> that is selectable.
    /// </summary>
    public interface IBindableSelectableChannel : ISelectableItem, IBindableChannel
    {
        /// <summary>
        /// Gets a command that handles a channel being selected.
        /// </summary>
        public RelayCommand SelectionCommand { get; }

        /// <summary>
        /// Selects the channel as appropriate 
        /// </summary>
        public void Select();
    }
}
