// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Interfaces;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Quarrel.Controls
{
    /// <summary>
    /// A Control for showing a user's avatar.
    /// </summary>
    public sealed partial class UserIconControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserIconControl"/> class.
        /// </summary>
        public UserIconControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the presence of the user should be shown.
        /// </summary>
        public bool ShowPresence { get; set; } = true;

        /// <summary>
        /// Gets or sets the size of the icon.
        /// </summary>
        public int Size { get; set; } = 40;

        /// <summary>
        /// Gets the user on display.
        /// </summary>
        public IBindableUser ViewModel => DataContext as IBindableUser;

        private void Image_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ShowPresence)
            {
                ImageSourceWithPresence?.Play();
            }
            else
            {
                ImageSourceWithoutPresence?.Play();
            }
        }

        private void Image_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (ShowPresence)
            {
                ImageSourceWithPresence?.Stop();
            }
            else
            {
                ImageSourceWithoutPresence?.Stop();
            }
        }

    }
}
