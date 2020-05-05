// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables.Abstract;

namespace Quarrel.ViewModels.Models.Bindables
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="Invite"/> model.
    /// </summary>
    public class BindableInvite : BindableModelBase<Invite>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableInvite"/> class.
        /// </summary>
        /// <param name="invite">The base <paramref name="invite"/> object.</param>
        public BindableInvite(Invite invite) : base(invite)
        {
        }

        /// <summary>
        /// Gets the url of the guild icon.
        /// </summary>
        public string IconUrl => $"https://cdn.discordapp.com/icons/{Model.Guild.Id}/{Model.Guild.Icon}.png?size=128";

        /// <summary>
        /// Updates bindings for unbound properties.
        /// </summary>
        public void UpdateBindings()
        {
            RaisePropertyChanged(nameof(IconUrl));
        }
    }
}
