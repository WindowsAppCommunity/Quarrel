using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using Windows.UI.Notifications;
using System.Diagnostics;
using Discord_UWP;
using Windows.UI.Xaml;
using Windows.Storage;
using Windows.Security.Credentials;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace DiscordBackgroundTask1
{
    public sealed class SocketFrame
    {
        [JsonProperty("op")]
        public int? Operation { get; set; }
        [JsonProperty("d")]
        public object Payload { get; set; }
        [JsonProperty("s")]
        public int? SequenceNumber { get; set; }
        [JsonProperty("t")]
        public string Type { get; set; }
    }
    public sealed class ReadState
    {
        public string last_message_id { get; set; }
        public string id { get; set; }
        public int mention_count { get; set; }
    }
    public sealed class Recipient
    {
        public string username { get; set; }
        public string id { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
        public bool bot { get; set; }
    }
    public sealed class PrivateChannel
    {
        public int type { get; set; }
        public IEnumerable<Recipient> recipients { get; set; }
        public string last_message_id { get; set; }
        public string id { get; set; }
        public string owner_id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
    }
    public sealed class User2
    {
        public string username { get; set; }
        public string id { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
    }
    public sealed class Relationship
    {
        public User2 user { get; set; }
        public int type { get; set; }
        public string id { get; set; }
    }

    public sealed class MainClass : IBackgroundTask
    {
        
        BackgroundTaskDeferral _deferral;
        bool FinishedTask = false;
        private MemoryStream _compressed;
        private DeflateStream _decompressor;
        private MessageWebSocket webSocket = new MessageWebSocket();
        private  DataWriter _dataWriter;
        string token = "";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            /*
            _dataWriter = new DataWriter(webSocket.OutputStream);
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");
            _deferral = taskInstance.GetDeferral();

            //GET THE LOGIN TOKEN
            var creds = (new PasswordVault()).FindAllByResource("Token").FirstOrDefault();
            creds.RetrievePassword();
            token = creds.Password;

            //CONNECT TO THE GATEWAY
            webSocket.MessageReceived += WebSocket_MessageReceived;
            await webSocket.ConnectAsync(new Uri("wss://gateway.discord.gg/?v=6&encoding=json&compress=zlib-stream"));
            _compressed = new MemoryStream();
            _decompressor = new DeflateStream(_compressed, CompressionMode.Decompress);
                
            
            //WAIT FOR THE READY EVENT TO BE RECEIVED
            while (!FinishedTask)
                await Task.Delay(250);
            _deferral.Complete();*/
        }
       
        private void WebSocket_MessageReceived(Windows.Networking.Sockets.MessageWebSocket sender, Windows.Networking.Sockets.MessageWebSocketMessageReceivedEventArgs args)
        {
            using (var datastr = args.GetDataStream().AsStreamForRead())
            using (var ms = new MemoryStream())
            {
                datastr.CopyTo(ms);
                ms.Position = 0;
                byte[] data = new byte[ms.Length];
                ms.Read(data, 0, (int)ms.Length);
                int index = 0;
                int count = data.Length;
                using (var decompressed = new MemoryStream())
                {
                    if (data[0] == 0x78)
                    {
                        _compressed.Write(data, index + 2, count - 2);
                        _compressed.SetLength(count - 2);
                    }
                    else
                    {
                        _compressed.Write(data, index, count);
                        _compressed.SetLength(count);
                    }

                    _compressed.Position = 0;
                    _decompressor.CopyTo(decompressed);
                    _compressed.Position = 0;
                    decompressed.Position = 0;
                    using (var reader = new StreamReader(decompressed))
                        OnMessageReceived(reader.ReadToEnd());
                }
            }
        }
        public async void OnMessageReceived(string message)
        {
            SocketFrame frame = JsonConvert.DeserializeObject<SocketFrame>(message);
            if (frame.Operation.HasValue){
                if(frame.Operation == 10){
                    //Hello
                    string identify = "\"token\":\"" + token + "\",\"properties\":{\"$os\":\"\", \"$browser\":\"\", \"$device\":\"\"}";
                    SendMessageAsync("IDENTIFY", identify, 2);
                }
                else if(frame.Type == "READY")
                {
                    //Ready event
                    var ready = JObject.Parse(message)["d"];
                    IList<JToken> json_readstates = ready["read_state"].Children().ToList();
                    Dictionary<string, ReadState> readstates = new Dictionary<string, ReadState>();
                    foreach (var readstate in json_readstates)
                    {
                        var rs = readstate.ToObject<ReadState>();
                        readstates.Add(rs.id, rs);
                    }
                        

                    IList<JToken> privatechannels = ready["private_channels"].Children().ToList();
                    foreach (var json_channel in privatechannels)
                    {
                        var channel = json_channel.ToObject<PrivateChannel>();
                        if (readstates.ContainsKey(channel.id) && readstates[channel.id].mention_count > 0)
                        {
                            //Unfortunately the channel must be sent as a string parameter, because windowsruntime
                            SendToast.UnreadDM(Newtonsoft.Json.JsonConvert.SerializeObject(channel), readstates[channel.id].mention_count, readstates[channel.id].last_message_id);
                        }
                    }
                    foreach (var json_relationship in ready["relationships"])
                    {
                        var relationship = json_relationship.ToObject<Relationship>();
                        if (relationship.type == 3)
                        {
                            //incoming friend request, show notification
                            SendToast.FriendRequest(relationship.user.username, relationship.user.avatar, relationship.user.id, relationship.id);
                        }
                    }
                }
            }
        }

        public async void SendMessageAsync(string name, string content, int opcode)
        {
            try
            {
                _dataWriter.WriteString(("{\"op\": "+ opcode+",\"d\": {" + content + "},\"t\":\"" + name + "\"}"));
                await _dataWriter.StoreAsync();
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

    }
}
