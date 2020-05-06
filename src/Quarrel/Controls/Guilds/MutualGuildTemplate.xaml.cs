// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Guilds;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Guilds
{
    /// <summary>
    /// Template to show a Guild mutual to a user.
    /// </summary>
    public sealed partial class MutualGuildTemplate : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MutualGuildTemplate"/> class.
        /// </summary>
        public MutualGuildTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the guild Data in template.
        /// </summary>
        public BindableMutualGuild ViewModel => DataContext as BindableMutualGuild;
    }
}
