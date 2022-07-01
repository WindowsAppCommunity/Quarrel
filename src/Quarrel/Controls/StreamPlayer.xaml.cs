// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Quarrel.Client;
using Quarrel.Services.Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Webrtc;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class StreamPlayer : UserControl
    {
        public ulong UserId { get; set; }
        
        private const int c_frameRateN = 30;
        private const int c_frameRateD = 1;
        
        public StreamPlayer()
        {
            this.InitializeComponent();
            Init();
        }

        async void Init()
        {
            uint iWidth = 1280;
            uint iHeight = 638;
            
            VideoEncodingProperties videoProperties = VideoEncodingProperties.CreateUncompressed(MediaEncodingSubtypes.Nv12, iWidth, iHeight);
            VideoStreamDescriptor videoDesc = new VideoStreamDescriptor(videoProperties);
            videoDesc.EncodingProperties.FrameRate.Numerator = c_frameRateN;
            videoDesc.EncodingProperties.FrameRate.Denominator = c_frameRateD;
            videoDesc.EncodingProperties.Bitrate = (uint)(c_frameRateN * c_frameRateD * iWidth * iHeight * 4);

            videoDesc.EncodingProperties.Width = iWidth;
            videoDesc.EncodingProperties.Height = iHeight;

            var mss = new MediaStreamSource(videoDesc);
            TimeSpan spanBuffer = new TimeSpan(0, 0, 0, 0, 250);
            mss.BufferTime = spanBuffer;
            mss.Starting += mss_Starting;
            mss.SampleRequested += mss_SampleRequested;

            MediaElement.SetMediaStreamSource(mss);
            MediaElement.Play();
        }

        async void mss_Starting(MediaStreamSource sender, MediaStreamSourceStartingEventArgs args)
        {
            var deferral = args.Request.GetDeferral();

            // TODO: defer starting event until first frame recieved rather than adding a delay here
            await Task.Delay(2000);

            args.Request.SetActualStartPosition(new TimeSpan(0));

            deferral.Complete();
        }

        void mss_SampleRequested(MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            DiscordService discordService = (DiscordService)App.Current.Services.GetRequiredService<IDiscordService>();
            foreach ((string key, WebrtcManager manager) in discordService.Streams)
            {
                if (key.EndsWith(UserId.ToString()))
                {
                    manager.GenerateSample(args.Request);
                    return;
                }

            }
        }
    }
}
