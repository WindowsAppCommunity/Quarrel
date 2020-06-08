// Copyright (c) Quarrel. All rights reserved.

using myTube.Playback.Handlers;
using RykenTube;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls
{
    /// <summary>
    /// Control to display YouTube videos with the myTube player.
    /// </summary>
    public sealed partial class MyTubeEmbedWrapper : UserControl
    {
        private static readonly DependencyProperty
            VidoeUriProperty = DependencyProperty.Register(
                nameof(VideoUrl),
                typeof(string),
                typeof(MyTubeEmbedWrapper),
                new PropertyMetadata(string.Empty, OnVidoeUriChanged));

        private static readonly DependencyProperty
            PlayProperty = DependencyProperty.Register(
                nameof(Play),
                typeof(bool),
                typeof(MyTubeEmbedWrapper),
                new PropertyMetadata(string.Empty, OnPlayChanged));

        private bool _atBeginning;

        private myTubeHandlerContainer _mediaHandler = new myTubeHandlerContainer();

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTubeEmbedWrapper"/> class.
        /// </summary>
        public MyTubeEmbedWrapper()
        {
            this.InitializeComponent();
            RykenPlayer.CurrentMediaHandler = _mediaHandler;
            SetupMytube(VideoUrl);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the video is playing.
        /// </summary>
        public bool Play
        {
            get => (bool)GetValue(PlayProperty);
            set => SetValue(PlayProperty, value);
        }

        /// <summary>
        /// Gets or sets the Video by url.
        /// </summary>
        public string VideoUrl
        {
            get => (string)GetValue(VidoeUriProperty);
            set => SetValue(VidoeUriProperty, value);
        }

        /// <summary>
        /// Diposes of the MyTube instance.
        /// </summary>
        public void Dispose()
        {
            _mediaHandler.Deregister();
        }

        private static void OnVidoeUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyTubeEmbedWrapper wrapper && e.NewValue is string value && wrapper.Play)
            {
                wrapper._atBeginning = true;
                wrapper.ChangeVideo(value);
            }
        }

        private static void OnPlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyTubeEmbedWrapper wrapper && e.NewValue is bool value)
            {
                if (value)
                {
                    if (wrapper._atBeginning)
                    {
                        _ = wrapper._mediaHandler.CurrentVideoHandler.Play();
                    }
                    else
                    {
                        wrapper.ChangeVideo(wrapper.VideoUrl);
                    }
                }
                else
                {
                    wrapper._atBeginning = false;
                    _ = wrapper._mediaHandler.CurrentVideoHandler.Pause();
                }
            }
        }

        private async void SetupMytube(string url)
        {
            await _mediaHandler.SetCurrentVideoHandler(new MediaPlayerHandler { UseMediaPlayerElement = false });

            // Disable the media transport controls
            _mediaHandler.CurrentVideoHandler.HandlesTransportControls = true;

            // Keep video loaded when ended
            _mediaHandler.CurrentVideoHandler.StopOnMediaEnded = false;
            var match = Regex.Match(url, ViewModels.Helpers.Constants.Regex.YouTubeURLRegex);
            if (match.Success)
            {
                await _mediaHandler.CurrentVideoHandler.OpenVideo(new YouTubeEntry { ID = !string.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[1].Value : match.Groups[2].Value }, YouTubeQuality.HD);
            }
        }

        private async void ChangeVideo(string url)
        {
            var match = Regex.Match(url, ViewModels.Helpers.Constants.Regex.YouTubeURLRegex);
            if (match.Success)
            {
                await _mediaHandler.CurrentVideoHandler.OpenVideo(
                    new YouTubeEntry
                    {
                        ID = !string.IsNullOrEmpty(match.Groups[1].Value)
                            ? match.Groups[1].Value
                            : match.Groups[2].Value,
                    }, YouTubeQuality.HD);
            }
        }
    }
}
