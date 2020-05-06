// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Messages
{
    /// <summary>
    /// A Control to show instead of an embed when attachments are hidden.
    /// </summary>
    public sealed partial class HiddenAttachment : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HiddenAttachment"/> class.
        /// </summary>
        public HiddenAttachment()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the DataContext as a VideoEmbed.
        /// </summary>
        public BindableAttachment ViewModel => DataContext as BindableAttachment;
    }
}
