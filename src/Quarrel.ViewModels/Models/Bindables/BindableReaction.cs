// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables.Abstract;

namespace Quarrel.ViewModels.Models.Bindables
{
    /// <summary>
    /// A Bindable wrapper of the <see cref="Reaction"/> model.
    /// </summary>
    public class BindableReaction : BindableModelBase<Reaction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableReaction"/> class.
        /// </summary>
        /// <param name="reaction">The base <see cref="Reaction"/> object.</param>
        public BindableReaction(Reaction reaction) : base(reaction)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the current user has used this reaction.
        /// </summary>
        public bool Me
        {
            get => Model.Me;
            set
            {
                Model.Me = value;
                RaisePropertyChanged(nameof(Me));
            }
        }

        /// <summary>
        /// Gets or sets how many people have used this reaction.
        /// </summary>
        public int Count
        {
            get => Model.Count;
            set
            {
                Model.Count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }
    }
}
