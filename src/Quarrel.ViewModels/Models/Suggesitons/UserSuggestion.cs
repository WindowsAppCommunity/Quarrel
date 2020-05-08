// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Users;

namespace Quarrel.ViewModels.Models.Suggesitons
{
    /// <summary>
    /// Represents a suggested user to mention in the message box.
    /// </summary>
    public class UserSuggestion : ISuggestion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSuggestion"/> class.
        /// </summary>
        /// <param name="user">The user recommended for mentioning.</param>
        public UserSuggestion(BindableGuildMember user)
        {
            User = user;
        }

        /// <summary>
        /// Gets the user recommended for mentioning.
        /// </summary>
        public BindableGuildMember User { get; }

        /// <inheritdoc/>
        public string Surrogate => string.Format("{0}#{1}", User.Model.User.Username, User.Model.User.Discriminator);
    }
}
