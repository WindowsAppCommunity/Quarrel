// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Interfaces;

namespace Quarrel.ViewModels.Models.Bindables.Guilds
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="Folder"/> model.
    /// </summary>
    public class BindableGuildFolder : BindableModelBase<Folder>, IGuildListItem
    {
        private bool _isCollapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableGuildFolder"/> class.
        /// </summary>
        /// <param name="model">The base <see cref="Folder"/> object.</param>
        public BindableGuildFolder(Folder model) : base(model)
        {
        }

        /// <inheritdoc/>
        public bool IsCollapsed
        {
            get => _isCollapsed;
            set => Set(ref _isCollapsed, value);
        }

        /// <inheritdoc/>
        public bool ShowUnread => false; // TODO: this
    }
}
