// Quarrel © 2022

using Quarrel.Bindables.Interfaces;
using Quarrel.Client.Models.Channels.Interfaces;

namespace Quarrel.Bindables.Channels.Interfaces
{
    /// <summary>
    /// An interface for wrapping an <see cref="IChannel"/> that is selectable.
    /// </summary>
    public interface IBindableSelectableChannel : ISelectableItem, IBindableChannel
    {
    }
}
