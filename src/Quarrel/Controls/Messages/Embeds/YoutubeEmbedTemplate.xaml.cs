using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using DiscordAPI.Models;
using myTube.Playback.Handlers;
using Ryken.UI;
using RykenTube;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Messages.Embeds
{
    public sealed partial class YoutubeEmbedTemplate : UserControl
    {
        /// <summary>
        /// This Control cannot be made mvvm as Mytube embed is not platform agnostic
        /// Maybe it would be possible to find a way to move this code out the codebehind
        /// but there would not be much point doing so as myTubeHandlerContainer and MediaPlayerHandler
        /// contain references to Windows.UI.Xaml
        /// </summary>
        
        public YoutubeEmbedTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                if (ViewModel?.Url != null)
                {
                    SetupMytube(ViewModel.Url);
                }
            };
        }

        async void SetupMytube(string url)
        {
            Regex YouTubeRegex = new Regex(@"(?:https:\/\/)?(?:(?:www\.)?youtube\.com\/watch\?.*?v=([\w\-]+)|youtu\.be\/([\w\-]+))", RegexOptions.Compiled);
            var match = YouTubeRegex.Match(url);
            if (match.Success)
            {
                if (RykenPlayer.CurrentMediaHandler == null)
                {
                    myTubeHandlerContainer mediaHandler = new myTubeHandlerContainer();
                    RykenPlayer.CurrentMediaHandler = mediaHandler;
                    await mediaHandler.SetCurrentVideoHandler(new MediaPlayerHandler { UseMediaPlayerElement = false });
                    mediaHandler.CurrentVideoHandler.HandlesTransportControls = true; // Disable the media transport controls
                    mediaHandler.CurrentVideoHandler.StopOnMediaEnded = false; // Keep video loaded when ended
                    await mediaHandler.CurrentVideoHandler.OpenVideo(new YouTubeEntry { ID = match.Groups[1].Value != "" ? match.Groups[1].Value : match.Groups[2].Value }, YouTubeQuality.HD);
                    await mediaHandler.CurrentVideoHandler.Pause();
                }
            }
        }

        public Embed ViewModel => DataContext as Embed;
    }
}
