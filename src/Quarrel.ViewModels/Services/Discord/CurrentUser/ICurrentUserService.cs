// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables.Users;

namespace Quarrel.ViewModels.Services.Discord.CurrentUser
{
    /// <summary>
    /// Manages the all information directly pertaining to the current user.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the current user as a <see cref="BindableUser"/>.
        /// </summary>
        BindableUser CurrentUser { get; }

        /// <summary>
        /// Gets the current user's discord settings.
        /// </summary>
        UserSettings CurrentUserSettings { get; }
    }
}
