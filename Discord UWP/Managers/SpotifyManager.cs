using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Quarrel.Managers
{
    public static class SpotifyManager
    {
        public static JSON.State SpotifyState = null;
        public static bool IsWebSocketOpen = false;
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
                [JsonProperty("Spotify-Connection-Id")]
                public string SpotifyConnectionId { get; set; }
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
                public string ReleaseDate { get; set; }

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

            public partial class AccessTokenRetrieval
            {
                public string access_token { get; set; }
            }
        }
        private static DataWriter _dataWriter;
        private async static Task SendPing()
        {
            try
            {
                _dataWriter.WriteString("{\"type\":\"ping\"}");
                await _dataWriter.StoreAsync();
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        private static string spotifyToken = "";
        public async static Task<bool> UpdateToken()
        {
            var creds = (new Windows.Security.Credentials.PasswordVault()).FindAllByResource("Token").FirstOrDefault();
            creds.RetrievePassword();
            using (HttpClient client = new HttpClient())
            {
                string url = "https://discordapp.com/api/v6/users/@me/connections/spotify/" + spotifyUsername + "/access-token";
                client.DefaultRequestHeaders.Add("Authorization", creds.Password);
                var resp = await client.GetAsync(url);
                try
                {
                    spotifyToken = JsonConvert.DeserializeObject<JSON.AccessTokenRetrieval>(await resp.Content.ReadAsStringAsync()).access_token;
                }
                catch
                { }
                return resp.IsSuccessStatusCode;
            }
        }
        public async static Task<bool> GetInitialPlayerStatus()
        {
            using (HttpClient client = new HttpClient())
            {
                //Ask about options
                await client.SendAsync(new HttpRequestMessage(new HttpMethod(HttpMethods.Options), "https://api.spotify.com/v1/me/player"));
            }
            using (HttpClient client = new HttpClient())
            {
                //Get current state
                string url = "https://api.spotify.com/v1/me/player";
                client.DefaultRequestHeaders.Add("authorization", "Bearer " + spotifyToken);
                var resp = await client.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    string content = await resp.Content.ReadAsStringAsync();
                    SpotifyState = JsonConvert.DeserializeObject<JSON.State>(content);
                    SpotifyStateUpdated?.Invoke(null, null);
                    return true;
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }
                else return true;
            }
        }

        public async static Task<bool> RequestMe()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.spotify.com/v1/me";
                client.DefaultRequestHeaders.Add("authorization", "Bearer " + spotifyToken);
                var resp = await client.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    SpotifyState = JsonConvert.DeserializeObject<JSON.State>(await resp.Content.ReadAsStringAsync());
                    return true;
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }
                else return true;
            }
        }


        private static string spotifyUsername;
        private static DispatcherTimer timer;
        public async static void Start(string token, string username)
        {

            spotifyToken = token;
            spotifyUsername = username;
            if (await GetInitialPlayerStatus())
            {
                //Token appears to be valid
                await ConnectToWebsocket();
            }
            else
            {
                //Token was NOT valid, update it
                if(await UpdateToken())
                {
                    //Token was correctly updated, get initial status again
                    if(await GetInitialPlayerStatus())
                    {
                        //Token is valid, continue
                        await ConnectToWebsocket();
                    }
                    else
                    {
                        //Give up
                    }
                }
            }

        }

        private static async void Timer_Tick(object sender, object e)
        {
            if (IsWebSocketOpen)
            {
                await SendPing();
                timer.Start();
            }
        }

        private static MessageWebSocket webSocket;
        private async static Task<bool> ConnectToWebsocket()
        {
            try
            {
                //Connect to websocket
                webSocket = new MessageWebSocket();
                webSocket.MessageReceived += WebSocket_MessageReceived;
                webSocket.Closed += WebSocket_Closed;
                webSocket.Control.MessageType = SocketMessageType.Utf8;
                webSocket.Control.DesiredUnsolicitedPongInterval = TimeSpan.FromHours(1);
                
                _dataWriter = new DataWriter(webSocket.OutputStream);
                //webSocket.SetRequestHeader("Cookie", "wp_access_token" + "=" + token);

                webSocket.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134");
                webSocket.SetRequestHeader("Origin", "https://discordapp.com");
                //webSocket.SetRequestHeader("Cookie", "sp_landing=http%3A%2F%2Fopen.spotify.com%2Ffollow%2F1%3Furi%3Dspotify%3Aartist%3A21egYD1eInY6bGFcniCRT1%26size%3Ddetail%26theme%3Dlight; optimizelyEndUserId=oeu1520023948221r0.4129836485713321; sp_dc=AQB0T7lCVDF6amYFHwBWAMQxBSRsG3aGHoiaTrD2djHnmnEiX6QpN7WKwt9VuMSePnoyRnvVSBlidnNQUgL9mSx6; optimizelySegments=%7B%226174980032%22%3A%22search%22%2C%226176630028%22%3A%22none%22%2C%226179250069%22%3A%22false%22%2C%226161020302%22%3A%22edge%22%7D; _gid=GA1.2.599514017.1526173434; spot=%7B%22t%22%3A1520023935%2C%22m%22%3A%22uk%22%2C%22p%22%3A%22open%22%7D; _ga=GA1.2.1695017186.1519606025; wp_sso_token=AQB0T7lCVDF6amYFHwBWAMQxBSRsG3aGHoiaTrD2djHnmnEiX6QpN7WKwt9VuMSePnoyRnvVSBlidnNQUgL9mSx6; sp_t=9d1572a2db85715c17f4e623e100522e; optimizelyBuckets=%7B%7D; sp_ab=%7B%7D");
                await webSocket.ConnectAsync(new Uri("wss://dealer.spotify.com/?access_token=" + spotifyToken));
                IsWebSocketOpen = true;
                stopwatch.Start();
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal,
    () =>
    {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(25);
        timer.Tick += Timer_Tick;
        timer.Start();
    });
                return true;
            }
            catch
            {
                //websocket connection failed
                return false;
            }
        }

        static Stopwatch stopwatch = new Stopwatch();
        private async static void WebSocket_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            //Websocket closed, burn everything down
            SpotifyState = null;
            IsWebSocketOpen = false;
            SpotifyStateUpdated?.Invoke(null, null);
        }

        private async static void WebSocket_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            if(timer != null)
            {
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    timer.Stop();
    timer.Start();
});
            }
            
            var dr = args.GetDataReader();
            JSON.Root root = Newtonsoft.Json.JsonConvert.DeserializeObject<JSON.Root>(dr.ReadString(dr.UnconsumedBufferLength));
            if (root.Type == "message")
            {
                if (root.Payloads != null)
                {
                    foreach (var payload in root.Payloads)
                    {
                        if (payload.Events != null)
                            foreach (var ev in payload.Events)
                            {
                                if (ev.Type == "PLAYER_STATE_CHANGED")
                                {
                                    
                                    SpotifyState = ev.Event.State;
                                    SpotifyStateUpdated?.Invoke(null, null);
                                }
                                else if(ev.Type == "DEVICE_STATE_CHANGED")
                                {
                                    //check for the spotify status again, maybe it's null (with invalid token verification)
                                    if (!await GetInitialPlayerStatus())
                                    {
                                        if (await UpdateToken())
                                            await GetInitialPlayerStatus();
                                        else //otherwise, just give up and burn the entire thing down
                                        { SpotifyState = null; SpotifyStateUpdated?.Invoke(null, null); }
                                    }
                                }
                            }
                    }
                }
                else if(root.Headers!= null && root.Headers.SpotifyConnectionId != null)
                {
                    using(HttpClient client = new HttpClient())
                    {
                        string url = "https://api.spotify.com/v1/me/notifications/player?connection_id=" + Uri.EscapeDataString(root.Headers.SpotifyConnectionId);
                        client.DefaultRequestHeaders.Add("Access-Control-Request-Method", "PUT");
                        client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "authorization");
                        client.DefaultRequestHeaders.Add("Referer", "https://discordapp.com/channels/@me");
                        client.DefaultRequestHeaders.Add("Origin", "https://discordapp.com");
                        await client.SendAsync(new HttpRequestMessage(HttpMethod.Options, url));
                    }
                    using (HttpClient client = new HttpClient())
                    {
                        string url = "https://api.spotify.com/v1/me/notifications/player?connection_id=" + Uri.EscapeDataString(root.Headers.SpotifyConnectionId);
                        client.DefaultRequestHeaders.Add("authorization", "Bearer " + spotifyToken);
                        client.DefaultRequestHeaders.Add("Origin", "https://discordapp.com");
                        await client.SendAsync(new HttpRequestMessage(HttpMethod.Put, url));
                    }
                }
            }
        }
    }
}
