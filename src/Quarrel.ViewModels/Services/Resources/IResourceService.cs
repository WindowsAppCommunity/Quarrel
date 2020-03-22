// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Services.Resources
{
    /// <summary>
    /// Forwards resources through MVVM.
    /// </summary>
    public interface IResourceService
    {
        /// <summary>
        /// Gets a resource from App.Current.Resources.
        /// </summary>
        /// <param name="resource">Resource name.</param>
        /// <returns>The resource from App.Current.Resources.</returns>
        object GetResource(string resource);

        /// <summary>
        /// Gets the int accent color for a user.
        /// </summary>
        /// <param name="user">The user to get the accent color for.</param>
        /// <returns>The user's int accent color.</returns>
        Task<int> GetUserAccentColor(User user);

        /// <summary>
        /// Gets the int color for a status.
        /// </summary>
        /// <param name="status">The status to get int color for.</param>
        /// <returns>The int color for the status.</returns>
        int GetStatusColor(string status);
    }
}
