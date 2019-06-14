using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DiscordAPI.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class GifvControl : UserControl
    {
        private DependencyProperty EmbedContentProperty;

        /// <summary>
        /// API Embed Content to display
        /// </summary>
        public Embed EmbedContent
        {
            get { return (Embed)GetValue(EmbedContentProperty); }
            set {
                mediaelement.AutoPlay = true;
                mediaelement.IsLooping = true;
                mediaelement.Source = new Uri(value.Video.Url);
                if (value.Video.Width < 400) mediaelement.MaxWidth = value.Video.Width;
                mediaelement.MaxHeight = value.Video.Height;
            }
        }

        public GifvControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Media finished loading
        /// </summary>
        private void mediaelement_MediaOpened(object sender, RoutedEventArgs e)
        {
            LoadingIndic.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            //Nothing to dipose
        }
    }
}
