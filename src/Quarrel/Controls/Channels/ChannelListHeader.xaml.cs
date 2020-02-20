// Copyright (c) Quarrel. All rights reserved.

using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Channels
{
    /// <summary>
    /// Guild Button as header for ChannelList.
    /// </summary>
    public sealed partial class ChannelListHeader : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelListHeader"/> class.
        /// </summary>
        public ChannelListHeader()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();

                // Update height
                // 48 without banner, 64 with
                if (ViewModel != null)
                {
                    if (ViewModel.Model.BannerUri == null)
                    {
                        rootButton.Height = 48;
                    }
                    else
                    {
                        rootButton.Height = 64;
                    }
                }
            };
        }

        /// <summary>
        /// Gets the current Guild.
        /// </summary>
        public BindableGuild ViewModel => DataContext as BindableGuild;

        /// <summary>
        /// Fades in Image when banner loads.
        /// </summary>
        private async void ImageEx_ImageExOpened(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExOpenedEventArgs e)
        {
            await Banner.Fade(1, 200).StartAsync();
        }
    }
}
