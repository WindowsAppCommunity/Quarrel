// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Views
{
    /// <summary>
    /// Control for FriendsList page on DM Guild.
    /// </summary>
    public sealed partial class FriendListControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendListControl"/> class.
        /// </summary>
        public FriendListControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the MainViewModel for the app.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
