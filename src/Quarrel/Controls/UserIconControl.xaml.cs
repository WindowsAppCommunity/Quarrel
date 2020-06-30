// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml;
using Quarrel.ViewModels.Models.Interfaces;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

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
            DataContextChanged += UserIconControl_DataContextChanged;
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
                (ImageWithPresence.ImageSource as BitmapImage)?.Play();
            }
            else
            {
                (ImageWithoutPresence.Source as BitmapImage)?.Play();
            }
        }

        private void Image_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (ShowPresence)
            {
                (ImageWithPresence.ImageSource as BitmapImage)?.Stop();
            }
            else
            {
                (ImageWithoutPresence.Source as BitmapImage)?.Stop();
            }
        }

        private void UserIconControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.Bindings.Update();
            if (sender is UserIconControl ctrl && ctrl.DataContext is IBindableUser context)
            {
                var img = new BitmapImage
                {
                    UriSource = new Uri(context.RawModel.AvatarUrl),
                    DecodePixelHeight = Size,
                    DecodePixelWidth = Size,
                    DecodePixelType = DecodePixelType.Logical,
                };
                if (ShowPresence)
                {
                    if (ImageWithPresence != null)
                    {
                        ImageWithPresence.ImageSource = img;
                    }
                }
                else if (ImageWithoutPresence != null)
                {
                    ImageWithoutPresence.Source = img;
                }
            }
            else
            {
                if (ImageWithoutPresence != null)
                {
                    ImageWithoutPresence.Source = null;
                }

                if (ImageWithPresence != null)
                {
                    ImageWithPresence.ImageSource = null;
                }
            }
        }

    }
}
