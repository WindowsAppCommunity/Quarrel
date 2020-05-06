// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels;
using Quarrel.ViewModels.Models.Bindables.Messages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Messages
{
    /// <summary>
    /// Flyout for a BindableMessageTemplate.
    /// </summary>
    public sealed partial class MessageFlyout : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFlyout"/> class.
        /// </summary>
        public MessageFlyout()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the message on display.
        /// </summary>
        public BindableMessage Message => DataContext as BindableMessage;

        /// <summary>
        /// Gets the MainViewModel for the app.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
