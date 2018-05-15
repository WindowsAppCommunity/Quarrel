using Discord_UWP.SharedModels;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class GifvControl : UserControl
    {
        private DependencyProperty EmbedContentProperty;

        public Embed EmbedContent
        {
            get { return (Embed)GetValue(EmbedContentProperty); }
            set {
                mediaelement.AutoPlay = true;
                mediaelement.Source = new Uri(value.Video.Url);
                if (value.Video.Width < 400) mediaelement.MaxWidth = value.Video.Width;
                mediaelement.MaxHeight = value.Video.Height;
            }
        }

        public GifvControl()
        {
            this.InitializeComponent();
        }
        private void mediaelement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaelement.Position = TimeSpan.Zero;
            mediaelement.Play();
        }

        private void mediaelement_MediaOpened(object sender, RoutedEventArgs e)
        {
            
            /*
            await mediaelement.Fade(1, 100).StartAsync();
            if (!Storage.Settings.GifsOnHover)
            {
                mediaelement.AutoPlay = true;
            }     
            else
            {
                mediaelement.AutoPlay = false;
                if (above)
                    mediaelement.Play();
            }
                */
            LoadingIndic.Visibility = Visibility.Collapsed;
        }

        private void content_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
        }

        bool above = false;
        private void mediaelement_PointerEntered(object sender, PointerRoutedEventArgs e)
        {/*
            above = true;
            if(e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse && Storage.Settings.GifsOnHover)
            {
                mediaelement.Position = TimeSpan.Zero;
                if(mediaelement.CurrentState == MediaElementState.Stopped)
                mediaelement.Play();
            }*/
        }

        private void mediaelement_PointerExited(object sender, PointerRoutedEventArgs e)
        {/*
            above = false;
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse && Storage.Settings.GifsOnHover)
            {
                mediaelement.Position = TimeSpan.Zero;
                mediaelement.Stop();
            }*/
        }

        private void mediaelement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

        private void mediaelement_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            /*
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse && Storage.Settings.GifsOnHover)
            {
                if (above)
                {
                    above = false;
                    mediaelement.Position = TimeSpan.Zero;
                    if (mediaelement.CurrentState == MediaElementState.Playing)
                        mediaelement.Stop();
                }
                else
                {
                    above = true;
                    mediaelement.Position = TimeSpan.Zero;
                    if (mediaelement.CurrentState == MediaElementState.Stopped)
                        mediaelement.Play();
                }

            }*/
        }
    }
}
