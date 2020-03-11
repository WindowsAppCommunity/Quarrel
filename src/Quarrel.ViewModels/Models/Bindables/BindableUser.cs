// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using System;

namespace Quarrel.ViewModels.Models.Bindables
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="User"/> model.
    /// </summary>
    public class BindableUser : BindableModelBase<User>, IEquatable<BindableUser>, IComparable<BindableUser>
    {
        private Presence _presence;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableUser"/> class.
        /// </summary>
        /// <param name="model">The base <see cref="User"/> object.</param>
        public BindableUser([NotNull] User model) : base(model)
        {
        }

        /// <summary>
        /// Gets or sets the user's presence.
        /// </summary>
        public Presence Presence
        {
            get => _presence;
            set => Set(ref _presence, value);
        }

        /// <inheritdoc/>
        public bool Equals(BindableUser other)
        {
            return Model.Id == other.Model.Id;
        }

        /// <inheritdoc/>
        public int CompareTo(BindableUser other)
        {
            return Model.Id.CompareTo(other.Model.Id);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Model.Id.GetHashCode();
        }
    }
}
