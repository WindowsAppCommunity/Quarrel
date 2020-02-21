// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.GitHub;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.GitHub
{
    /// <summary>
    /// Template shown for Developers in CreditPage.
    /// </summary>
    public sealed partial class DeveloperTemplate : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeveloperTemplate"/> class.
        /// </summary>
        public DeveloperTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the developer being shown.
        /// </summary>
        public BindableDeveloper ViewModel => DataContext as BindableDeveloper;
    }
}
