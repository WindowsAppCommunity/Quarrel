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
using Windows.Foundation.Collections;

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
    public sealed class Channel
    {
        public string last_message_id { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }
    public sealed class Guild
    {
        public IEnumerable<Channel> channels { get; set; }
        public string id { get; set; }
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
            _compressed = new MemoryStream();
            _decompressor = new DeflateStream(_compressed, CompressionMode.Decompress);
            _dataWriter = new DataWriter(webSocket.OutputStream);
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");
            _deferral = taskInstance.GetDeferral();

            //GET THE LOGIN TOKEN
            try
            {
            var creds = (new PasswordVault()).FindAllByResource("Token").FirstOrDefault();
            creds.RetrievePassword();
            token = creds.Password;
            }
            catch (Exception ex)
            {
                UpdateLastRunStatus("We couldn't retrieve the login token last time the background task ran ("+ex.Message+")");
                return;
            }


            //CONNECT TO THE GATEWAY
            webSocket.MessageReceived += WebSocket_MessageReceived;
            try
            {

                await webSocket.ConnectAsync(new Uri("wss://gateway.discord.gg/?v=6&encoding=json&compress=zlib-stream"));
            }
            catch(Exception ex)
            {

                UpdateLastRunStatus("On the last run, the task failed to connect to the gateway (" + ex.Message + ")");
                return;
            }
            
                
            
            //WAIT FOR THE READY EVENT TO BE RECEIVED
            while (!FinishedTask)
                await Task.Delay(250);
            _deferral.Complete();
        }
       
        private void WebSocket_MessageReceived(Windows.Networking.Sockets.MessageWebSocket sender, Windows.Networking.Sockets.MessageWebSocketMessageReceivedEventArgs args)
        {
            try
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
                        if (decompressed == null) return;
                        _decompressor.CopyTo(decompressed);
                        _compressed.Position = 0;
                        decompressed.Position = 0;
                        using (var reader = new StreamReader(decompressed))
                            OnMessageReceived(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception)
            {
                FinishedTask = true;
                UpdateLastRunStatus("There was a problem while decrypting a websocket message on the last run");
            }
        }
        public async void OnMessageReceived(string message)
        {
            try
            {
                SocketFrame frame = JsonConvert.DeserializeObject<SocketFrame>(message);
                if (frame.Operation.HasValue)
                {
                    if (frame.Operation == 10)
                    {
                        //Hello
                        string identify = "\"token\":\"" + token + "\",\"properties\":{\"$os\":\"\", \"$browser\":\"\", \"$device\":\"\"}";
                        SendMessageAsync("IDENTIFY", identify, 2);
                    }
                    else if (frame.Type == "READY")
                    {
                        try
                        {
                            //Ready event
                            var ready = JObject.Parse(message)["d"];
                            IList<JToken> json_readstates = ready["read_state"].Children().ToList();
                            Dictionary<string, ReadState> readstates = new Dictionary<string, ReadState>();
                            int totalmentioncount = 0;
                            foreach (var readstate in json_readstates)
                            {
                                var rs = readstate.ToObject<ReadState>();
                                readstates.Add(rs.id, rs);
                                totalmentioncount += rs.mention_count;
                            }


                            IList<JToken> privatechannels = ready["private_channels"].Children().ToList();
                            
                            if (GetSetting("bgNotifyDM"))
                                foreach (var json_channel in privatechannels)
                                {
                                    var channel = json_channel.ToObject<PrivateChannel>();
                                    if(channel!=null && channel.id != null)
                                    if (readstates.ContainsKey(channel.id) && readstates[channel.id].mention_count > 0)
                                    {
                                        //Unfortunately the channel must be sent as a string parameter, because windowsruntime
                                        int notificationcount = GetNotificatinoCount("c" + channel.id, readstates[channel.id].mention_count);
                                        if (notificationcount > 0)
                                        {
                                            SendToast.UnreadDM(Newtonsoft.Json.JsonConvert.SerializeObject(channel), readstates[channel.id].mention_count, readstates[channel.id].last_message_id);
                                            UpdateNotificationState("c" + channel.id, readstates[channel.id].mention_count.ToString());
                                        }
                                    }
                                }
                            if (GetSetting("bgNotifyFriend"))
                                foreach (var json_relationship in ready["relationships"])
                                {
                                    var relationship = json_relationship.ToObject<Relationship>();
                                    if(relationship != null)
                                    if (relationship.type == 3)
                                    {
                                        //incoming friend request, show notification
                                        if (ShouldShowNotification("r" + relationship.user.id, relationship.id))
                                        {
                                            SendToast.FriendRequest(relationship.user.username, relationship.user.avatar, relationship.user.id, relationship.id);
                                            UpdateNotificationState("r" + relationship.user.id, relationship.id);
                                        }
                                    }
                                }
                            if (GetSetting("bgNotifyMention"))
                                foreach (var json_guild in ready["guilds"])
                                {
                                    var guild = json_guild.ToObject<Guild>();
                                    if(guild != null && guild.channels != null && (GetSetting("bgNotifyMutedMention") || !ready["user_guild_settings"].ToObject<IEnumerable<DiscordAPI.SharedModels.GuildSetting>>().FirstOrDefault(x => x.GuildId == guild.id).Muted))
                                    foreach (var channel in guild.channels)
                                    {
                                        if (readstates.ContainsKey(channel.id) && readstates[channel.id].mention_count > 0)
                                        {
                                            int notificationcount = GetNotificatinoCount("c" + channel.id, readstates[channel.id].mention_count);
                                            if (notificationcount>0)
                                            {
                                                SendToast.NewMention(guild.icon, guild.id, guild.name, channel.name, channel.id, readstates[channel.id].mention_count, readstates[channel.id].last_message_id);
                                                UpdateNotificationState("c" + channel.id, readstates[channel.id].mention_count.ToString());
                                            }
                                        }
                                    }
                                }
                            UpdateLastRun();
                        }
                        catch(Exception ex)
                        {
                            UpdateLastRunStatus("There was an issue while processing the \"READY\" gateway event on the last run (" + ex.Source + ": "+ ex.Message+")");
                        }
                        webSocket.Close(1000, "");
                    }
                }
            }
            catch(Exception ex)
            {
                UpdateLastRunStatus("There was a general error while processing a websocket message on the last run (" + ex.Message + ")");
            }
        }
        private bool GetSetting(string name)
        {
            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(name))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(name, true);
            return (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values[name];
        }
        private void UpdateLastRun()
        {
            UpdateLastRunStatus("");
            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("bgTaskLastrun"))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add("bgTaskLastrun", DateTimeOffset.Now.ToUnixTimeSeconds());
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["bgTaskLastrun"] = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        private void UpdateLastRunStatus(string message)
        {
            if(!string.IsNullOrWhiteSpace(message))
                message = message + " (" + DateTime.Now.ToString() + ")";
            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("bgTaskLastrunStatus"))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add("bgTaskLastrunStatus", message);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["bgTaskLastrunStatus"] = message;
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
                UpdateLastRunStatus("The background task ran into an issue while sending a message to the gateway");
            }
        }

        public void UpdateNotificationState(string id, string timestamp)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("NotificationStates"))
                ApplicationData.Current.LocalSettings.Values.Add("NotificationStates", "{}");
            var nrs = ApplicationData.Current.LocalSettings.Values["NotificationStates"];
            var nrs2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(nrs.ToString());
            if (nrs2 != null && nrs2.ContainsKey(id))
            {
                nrs2[id] = timestamp;
            }
            else
            {
                if (nrs2 == null) nrs2 = new Dictionary<string, string>();
                nrs2.Add(id, timestamp);
            }
            ApplicationData.Current.LocalSettings.Values["NotificationStates"] = JsonConvert.SerializeObject(nrs2);
        }

        /// <summary>
        /// Returns the notification count which should be displayed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notificationcount"></param>
        /// <returns></returns>
        public int GetNotificatinoCount(string id, int notificationcount)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("NotificationStates"))
                ApplicationData.Current.LocalSettings.Values.Add("NotificationStates", "{}");
            var nrs = ApplicationData.Current.LocalSettings.Values["NotificationStates"];
            var nrs2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(nrs.ToString());
            if (nrs2 == null) return notificationcount;
            if (nrs2.ContainsKey(id))
            {
                int storednotificationcount;
                if (Int32.TryParse(nrs2[id], out storednotificationcount) && notificationcount > storednotificationcount)
                    return storednotificationcount-notificationcount;
                else
                    return 0;
            }
            else
                return notificationcount;
        }
        public bool ShouldShowNotification(string id, string notificationcount)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("NotificationStates"))
                ApplicationData.Current.LocalSettings.Values.Add("NotificationStates", "{}");
            var nrs = ApplicationData.Current.LocalSettings.Values["NotificationStates"];
            
            var nrs2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(nrs.ToString());
            if (nrs2 == null) return true;
            if (nrs2.ContainsKey(id))
            {
                if (nrs2[id] != notificationcount)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
    }
}
