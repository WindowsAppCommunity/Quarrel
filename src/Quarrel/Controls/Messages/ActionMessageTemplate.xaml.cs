// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Messages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Messages
{
    /// <summary>
    /// Message Template for actions (like pinned message).
    /// </summary>
    public sealed partial class ActionMessageTemplate : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMessageTemplate"/> class.
        /// </summary>
        public ActionMessageTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the message containing action data.
        /// </summary>
        public BindableMessage ViewModel => DataContext as BindableMessage;
    }
}
