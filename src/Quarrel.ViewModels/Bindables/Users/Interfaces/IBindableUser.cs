// Quarrel © 2022

using Quarrel.Client.Models.Users;
using System;

namespace Quarrel.Bindables.Users.Interfaces
{
    /// <summary>
    /// An interface for a wrapped <see cref="Client.Models.Users.User"/>.
    /// </summary>
    public interface IBindableUser
    {
        /// <summary>
        /// Gets the wrapped <see cref="Client.Models.Users.User"/>.
        /// </summary>
        User User { get; }

        /// <summary>
        /// Gets the url of the user's avatar.
        /// </summary>
        string? AvatarUrl {get; }

        /// <summary>
        /// Gets the url of the user's avatar as a <see cref="Uri"/>.
        /// </summary>
        Uri? AvatarUri { get; }
    }
}
