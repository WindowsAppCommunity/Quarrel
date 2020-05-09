// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Messages;
using Quarrel.ViewModels.Services.Discord.Rest;
using Windows.UI.Xaml.Controls.Primitives;
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

        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
