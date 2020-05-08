// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Channels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Channels
{
    /// <summary>
    /// Control to show a member of a Voice Channel.
    /// </summary>
    public sealed partial class VoiceMemberControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceMemberControl"/> class.
        /// </summary>
        public VoiceMemberControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the user showing.
        /// </summary>
        public BindableVoiceUser ViewModel => DataContext as BindableVoiceUser;
    }
}
