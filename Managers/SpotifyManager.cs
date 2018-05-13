using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Discord_UWP.Managers
{
    public static class SpotifyManager
    {
        public static JSON.State SpotifyState = null;
        public static event EventHandler SpotifyStateUpdated;
        public class JSON
        {
            public partial class Root
            {
                [JsonProperty("headers")]
                public Headers Headers { get; set; }

                [JsonProperty("payloads")]
                public Payload[] Payloads { get; set; }

                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("uri")]
                public string Uri { get; set; }
            }

            public partial class Headers
            {
                [JsonProperty("content-type")]
                public string ContentType { get; set; }
            }

            public partial class Payload
            {
                [JsonProperty("events")]
                public EventElement[] Events { get; set; }
            }

            public partial class EventElement
            {
                [JsonProperty("source")]
                public string Source { get; set; }

                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("uri")]
                public object Uri { get; set; }

                [JsonProperty("href")]
                public string Href { get; set; }

                [JsonProperty("event")]
                public EventEvent Event { get; set; }

                [JsonProperty("user")]
                public User User { get; set; }
            }

            public partial class EventEvent
            {
                [JsonProperty("event_id")]
                public long EventId { get; set; }

                [JsonProperty("state")]
                public State State { get; set; }
            }

            public partial class State
            {
                [JsonProperty("timestamp")]
                public long Timestamp { get; set; }

                [JsonProperty("progress_ms")]
                public long ProgressMs { get; set; }

                [JsonProperty("is_playing")]
                public bool IsPlaying { get; set; }

                [JsonProperty("item")]
                public Item Item { get; set; }

                [JsonProperty("context")]
                public object Context { get; set; }

                [JsonProperty("device")]
                public Device Device { get; set; }

                [JsonProperty("repeat_state")]
                public string RepeatState { get; set; }

                [JsonProperty("shuffle_state")]
                public bool ShuffleState { get; set; }
            }

            public partial class Device
            {
                [JsonProperty("id")]
                public string Id { get; set; }

                [JsonProperty("is_active")]
                public bool IsActive { get; set; }

                [JsonProperty("is_restricted")]
                public bool IsRestricted { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("volume_percent")]
                public long VolumePercent { get; set; }
            }

            public partial class Item
            {
                [JsonProperty("album")]
                public Album Album { get; set; }

                [JsonProperty("artists")]
                public Artist[] Artists { get; set; }

                [JsonProperty("disc_number")]
                public long DiscNumber { get; set; }

                [JsonProperty("duration_ms")]
                public long DurationMs { get; set; }

                [JsonProperty("explicit")]
                public bool Explicit { get; set; }

                [JsonProperty("external_ids")]
                public ExternalIds ExternalIds { get; set; }

                [JsonProperty("external_urls")]
                public ExternalUrls ExternalUrls { get; set; }

                [JsonProperty("href")]
                public string Href { get; set; }

                [JsonProperty("id")]
                public string Id { get; set; }

                [JsonProperty("is_local")]
                public bool IsLocal { get; set; }

                [JsonProperty("is_playable")]
                public bool IsPlayable { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("popularity")]
                public long Popularity { get; set; }

                [JsonProperty("preview_url")]
                public object PreviewUrl { get; set; }

                [JsonProperty("track_number")]
                public long TrackNumber { get; set; }

                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("uri")]
                public string Uri { get; set; }
            }

            public partial class Album
            {
                [JsonProperty("album_type")]
                public string AlbumType { get; set; }

                [JsonProperty("artists")]
                public Artist[] Artists { get; set; }

                [JsonProperty("external_urls")]
                public ExternalUrls ExternalUrls { get; set; }

                [JsonProperty("href")]
                public string Href { get; set; }

                [JsonProperty("id")]
                public string Id { get; set; }

                [JsonProperty("images")]
                public Image[] Images { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("release_date")]
                public DateTimeOffset ReleaseDate { get; set; }

                [JsonProperty("release_date_precision")]
                public string ReleaseDatePrecision { get; set; }

                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("uri")]
                public string Uri { get; set; }
            }

            public partial class Artist
            {
                [JsonProperty("external_urls")]
                public ExternalUrls ExternalUrls { get; set; }

                [JsonProperty("href")]
                public string Href { get; set; }

                [JsonProperty("id")]
                public string Id { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("uri")]
                public string Uri { get; set; }
            }

            public partial class ExternalUrls
            {
                [JsonProperty("spotify")]
                public string Spotify { get; set; }
            }

            public partial class Image
            {
                [JsonProperty("height")]
                public long Height { get; set; }

                [JsonProperty("url")]
                public string Url { get; set; }

                [JsonProperty("width")]
                public long Width { get; set; }
            }

            public partial class ExternalIds
            {
                [JsonProperty("isrc")]
                public string Isrc { get; set; }
            }

            public partial class User
            {
                [JsonProperty("id")]
                public string Id { get; set; }
            }
        }
        public async static void StartGateway(string token)
        {
            //Generate a random Sec-WebSocket-Key, with 16 ASCII characters between 32 and 127, then encode to base64
            Random rnd = new Random();
            byte[] bytes = new byte[16];
            for(var i=0; i < 16; i++)
            {
                bytes[i] = Convert.ToByte(rnd.Next(32, 127));
            }
            var websocketKey = Convert.ToBase64String(bytes);

            //Connect to websocket
            MessageWebSocket webSocket = new MessageWebSocket();
            webSocket.MessageReceived += WebSocket_MessageReceived; ;
            await webSocket.ConnectAsync(new Uri("https://dealer.spotify.com/?access_token="+token));
        }

        private static void WebSocket_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            var dr = args.GetDataReader();
            JSON.Root root = Newtonsoft.Json.JsonConvert.DeserializeObject<JSON.Root>(dr.ReadString(dr.UnconsumedBufferLength));
            if (root.Type == "message")
            {
                foreach(var payload in root.Payloads)
                {
                    if(payload.Events != null)
                        foreach(var ev in payload.Events)
                        {
                            if(ev.Type == "PLAYER_STATE_CHANGED")
                            {
                                SpotifyState = ev.Event.State;
                                SpotifyStateUpdated?.Invoke(null, null);
                            }
                        }
                }
            }
        }
    }
}
