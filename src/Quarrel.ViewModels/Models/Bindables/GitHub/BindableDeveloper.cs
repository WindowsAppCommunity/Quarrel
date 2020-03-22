// Copyright (c) Quarrel. All rights reserved.

using GitHubAPI.Models;
using Quarrel.ViewModels.Models.Bindables.Abstract;

namespace Quarrel.ViewModels.Models.Bindables.GitHub
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="User"/> model.
    /// </summary>
    public class BindableDeveloper : BindableModelBase<User>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableDeveloper"/> class.
        /// </summary>
        /// <param name="user">The developer's user object.</param>
        /// <param name="contributor">The developers contributor object.</param>
        public BindableDeveloper(User user, Contributor contributor) : base(user)
        {
            Contributor = contributor;
        }

        /// <summary>
        /// Gets or sets the developer's information as a contributor to Quarrel.
        /// </summary>
        public Contributor Contributor { get; set; }
    }
}
