// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using System.ComponentModel;

namespace Quarrel.ViewModels.Models.Interfaces
{
    /// <summary>
    /// An interface for all bindable user objects.
    /// </summary>
    public interface IBindableUser : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the raw <see cref="User"/> type for the user.
        /// </summary>
        User RawModel { get; }

        /// <summary>
        /// Gets the presence of the user.
        /// </summary>
        Presence Presence { get; }
    }
}
