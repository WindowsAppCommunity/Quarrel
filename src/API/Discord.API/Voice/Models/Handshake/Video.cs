// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace Discord.API.Voice.Models.Handshake
{
    internal record Video
    {
        internal record VideoStream
        {
            internal record Resolution
            {
                [JsonPropertyName("width")]
                public int Width { get; set; }
                
                [JsonPropertyName("height")]
                public int Height { get; set; }
                
                [JsonPropertyName("type")]
                public string Type { get; set; }
            }

            [JsonPropertyName("active")]
            public bool Active { get; set; }

            [JsonPropertyName("max_bitrate")]
            public int MaxBitrate { get; set; }

            [JsonPropertyName("max_framerate")]
            public int MaxFramerate { get; set; }

            [JsonPropertyName("max_resolution")]
            public Resolution MaxResolution { get; set; }
            
            [JsonPropertyName("quality")]
            public int Quality { get; set; }

            [JsonPropertyName("rid")]
            public string Rid { get; set; }

            [JsonPropertyName("rtx_ssrc")]
            public uint RtxSSRC { get; set; }

            [JsonPropertyName("ssrc")]
            public uint SSRC { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }
        }
        
        [JsonPropertyName("audio_ssrc")]
        public uint AudioSSRC { get; set; }

        [JsonPropertyName("rtx_ssrc")]
        public uint RtxSSRC { get; set; }            
        
        [JsonPropertyName("video_ssrc")]
        public uint VideoSSRC { get; set; }
        
        [JsonPropertyName("user_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong UserId { get; set; }

        [JsonPropertyName("streams")]
        public VideoStream[] Streams { get; set; }
    }
}
