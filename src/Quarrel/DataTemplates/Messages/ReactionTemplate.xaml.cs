// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels;

namespace Quarrel.DataTemplates.Messages
{
    /// <summary>
    /// A collection of Data Templates for Reaction displaying.
    /// </summary>
    public partial class ReactionTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactionTemplate"/> class.
        /// </summary>
        public ReactionTemplate()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the app's MainViewModel.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
