using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Services.Discord.Friends
{
    /// <summary>
    /// Manages all relationship status with the current user.
    /// </summary>
    public interface IFriendsService
    {
        /// <summary>
        /// Gets a hashed collection of friends, by user id.
        /// </summary>
        IDictionary<string, BindableFriend> Friends { get; }

        /// <summary>
        /// Gets a hashed collection of DM Members, by user id.
        /// </summary>
        IDictionary<string, BindableGuildMember> DMUsers { get; }
    }
}
