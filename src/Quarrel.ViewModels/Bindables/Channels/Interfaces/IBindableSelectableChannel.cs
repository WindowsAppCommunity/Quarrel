// Quarrel © 2022

using Discord.API.Models.Channels.Interfaces;
using Quarrel.Bindables.Interfaces;

namespace Quarrel.Bindables.Channels.Interfaces
{
    /// <summary>
    /// An interface for wrapping an <see cref="IChannel"/> that is selectable.
    /// </summary>
    public interface IBindableSelectableChannel : ISelectableItem, IBindableChannel
    {
    }
}
