// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Views
{
    /// <summary>
    /// Control to handle GuildList.
    /// </summary>
    public sealed partial class GuildListControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuildListControl"/> class.
        /// </summary>
        public GuildListControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the MainViewModel for the app.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
