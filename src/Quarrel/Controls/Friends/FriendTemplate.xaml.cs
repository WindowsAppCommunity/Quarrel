// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Users;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Friends
{
    /// <summary>
    /// Template shown in FriendListControl for Friend items.
    /// </summary>
    public sealed partial class FriendTemplate : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendTemplate"/> class.
        /// </summary>
        public FriendTemplate()
        {
            this.InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets friend displayed as bindable object.
        /// </summary>
        public BindableFriend ViewModel => DataContext as BindableFriend;
    }
}
