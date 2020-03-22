// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Services.Discord.Presence
{
    /// <summary>
    /// Manages the presence of all users.
    /// </summary>
    public interface IPresenceService
    {
        /// <summary>
        /// Gets a user's presence by user id.
        /// </summary>
        /// <param name="userId">User's id.</param>
        /// <returns>The user's presence or <see langword="null"/> default.</returns>
        DiscordAPI.Models.Presence GetUserPrecense(string userId);

        /// <summary>
        /// Sets a user's presence.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="presence">The user's new presence.</param>
        void UpdateUserPrecense(string userId, DiscordAPI.Models.Presence presence);
    }
}
