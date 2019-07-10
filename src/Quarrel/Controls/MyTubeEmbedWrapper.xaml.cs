using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using myTube;
using myTube.Playback.Handlers;
using RykenTube;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class MyTubeEmbedWrapper : UserControl
    {
        public static readonly DependencyProperty
            VidoeUriProperty = DependencyProperty.Register("VideoUri", typeof(string),
                typeof(MyTubeEmbedWrapper), new PropertyMetadata("", OnVidoeUriChanged));
        public string VideoUri
        {
            get => (string)GetValue(VidoeUriProperty);
            set => SetValue(VidoeUriProperty, value);
        }

        public bool startedPlaying;

        private static void OnVidoeUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is MyTubeEmbedWrapper wrapper && e.NewValue is string value && wrapper.Play)
            {
                wrapper.startedPlaying = true;
                wrapper.ChangeVideo(value);
            }
        }

        public static readonly DependencyProperty
            PlayeProperty = DependencyProperty.Register("Play", typeof(bool),
                typeof(MyTubeEmbedWrapper), new PropertyMetadata("", OnPlayChanged));
        public bool Play
        {
            get => (bool)GetValue(PlayeProperty);
            set => SetValue(PlayeProperty, value);
        }
        private static void OnPlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is MyTubeEmbedWrapper wrapper && e.NewValue is bool value)
            {
                if (value)
                {
                    if (wrapper.startedPlaying)
                    {
                        _ = wrapper.mediaHandler.CurrentVideoHandler.Play();
                    }
                    else
                    {
                        wrapper.ChangeVideo(wrapper.VideoUri);
                    }
                }
                else
                {
                    wrapper.startedPlaying = false;
                    _ = wrapper.mediaHandler.CurrentVideoHandler.Pause();
                }
            }
        }

        private myTubeHandlerContainer mediaHandler = new myTubeHandlerContainer();
        public MyTubeEmbedWrapper()
        {
            this.InitializeComponent();
            RykenPlayer.CurrentMediaHandler = mediaHandler;
            SetupMytube(VideoUri);
        }

        async void SetupMytube(string url)
        {

                await mediaHandler.SetCurrentVideoHandler(new MediaPlayerHandler {UseMediaPlayerElement = false});
                mediaHandler.CurrentVideoHandler.HandlesTransportControls =
                    true; // Disable the media transport controls
                mediaHandler.CurrentVideoHandler.StopOnMediaEnded = false; // Keep video loaded when ended
            

            var match = Regex.Match(url, Helpers.Constants.Regex.YouTubeRegex);
            if (match.Success)
            {
                await mediaHandler.CurrentVideoHandler.OpenVideo(new YouTubeEntry { ID = !string.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[1].Value : match.Groups[2].Value }, YouTubeQuality.HD);
            }
        }


        async void ChangeVideo(string url)
        {
            var match = Regex.Match(url, Helpers.Constants.Regex.YouTubeRegex);
            if (match.Success)
            {
                await mediaHandler.CurrentVideoHandler.OpenVideo(
                    new YouTubeEntry
                    {
                        ID = !string.IsNullOrEmpty(match.Groups[1].Value)
                            ? match.Groups[1].Value
                            : match.Groups[2].Value
                    }, YouTubeQuality.HD);
            }
        }
    }
}
