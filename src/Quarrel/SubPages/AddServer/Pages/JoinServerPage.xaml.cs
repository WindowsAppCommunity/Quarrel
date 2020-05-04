// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.SubPages.AddServer.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.AddServer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JoinServerPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinServerPage"/> class.
        /// </summary>
        public JoinServerPage()
        {
            this.InitializeComponent();

            DataContext = new JoinServerPageViewModel();
        }

        /// <summary>
        /// Gets the data for joining a server.
        /// </summary>
        public JoinServerPageViewModel ViewModel => DataContext as JoinServerPageViewModel;
    }
}
