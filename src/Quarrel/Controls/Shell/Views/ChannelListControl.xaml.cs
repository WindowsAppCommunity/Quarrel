// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Views
{
    /// <summary>
    /// Control to handle ChannelLsit.
    /// </summary>
    public sealed partial class ChannelListControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelListControl"/> class.
        /// </summary>
        public ChannelListControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the MainViewModel for the app.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
