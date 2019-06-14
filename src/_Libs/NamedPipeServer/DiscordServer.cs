using DiscordPipeImpersonator.Payload;
using NamedPipeWrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NamedPipeServer.QuarrelAppService;

namespace DiscordPipeImpersonator
{
    public class DiscordPipeServer
    {
        public enum ConnectionState { Disconnected, Connecting, Connected }
        public event EventHandler<ConnectionState> ConnectionUpdate;
        public event EventHandler<Game> MessageReceived;
        public event EventHandler<string> SetAppId;

        public class HandshakeFrame
        {
            public string client_id { get; set; }
        }

        public partial class FrameMessage
        {
            [JsonProperty("cmd")]
            public string Cmd { get; set; }

            [JsonProperty("args")]
            public FrameArgs Args { get; set; }

            [JsonProperty("nonce")]
            public string Nonce { get; set; }
        }

        public partial class FrameArgs
        {
            [JsonProperty("pid")]
            public long Pid { get; set; }

            [JsonProperty("activity")]
            public Game Activity { get; set; }
        }
        public class Frame
        {
            public string message { get; set; }
            public uint opcode { get; set; }
            private bool TryReadUInt32(Stream stream, out uint value)
            {
                //Read the bytes available to us
                byte[] bytes = new byte[4];
                int cnt = stream.Read(bytes, 0, bytes.Length);

                //Make sure we actually have a valid value
                if (cnt != 4)
                {
                    value = default(uint);
                    return false;
                }

                //Flip the endianess if required then convert it to a number
                //if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                value = BitConverter.ToUInt32(bytes, 0);
                return true;
            }
            private int Min(int a, uint b)
            {
                if (b >= a) return a;
                return (int)b;
            }

            public bool ReadFrame(Stream stream)
            {
                //Try to read the opcode
                uint op;
                if (!TryReadUInt32(stream, out op))
                    return false;

                //Try to read the length
                uint len;
                if (!TryReadUInt32(stream, out len))
                    return false;

                uint readsRemaining = len;

                //Read the contents
                using (var mem = new MemoryStream())
                {
                    byte[] buffer = new byte[Min(2048, len)]; // read in chunks of 2KB
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, Min(buffer.Length, readsRemaining))) > 0)
                    {
                        readsRemaining -= len;
                        mem.Write(buffer, 0, bytesRead);
                    }

                    byte[] result = mem.ToArray();
                    if (result.LongLength != len)
                        return false;

                    opcode = op;
                    message = UTF8Encoding.UTF8.GetString(result);
                    return true;
                }

                //fun
                //if (a != null) { do { yield return true; switch (a) { case 1: await new Task(); default: lock (obj) { foreach (b in c) { for (int d = 0; d < 1; d++) { a++; } } } while (a is typeof(int) || (new Class()) != null) } goto MY_LABEL;

            }
        }
        private bool abort = false;
        public DiscordPipeServer()
        {
            
        }
        public void Abort()
        {
            abort = true;
        }
        NamedPipeWrapper.NamedPipeServer server;
        public async void Start()
        {
            server = new NamedPipeWrapper.NamedPipeServer("discord-ipc-0");
            server.StartListen();
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            Console.WriteLine("DiscordPipeServer is listening");
            while (!abort)
            {
                var result = await server.AwaitSingleMessageAsync<PipeFrame>();
                var frame = (PipeFrame)result.MessageObject;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("NEW FRAME:");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Opcode=" + frame.Opcode);
                Console.WriteLine("Message=" + frame.Message);
                Console.ForegroundColor = ConsoleColor.White;
                HandleMessage(frame);
                /*PipeFrame frame = new PipeFrame();
                using(var stream = new MemoryStream(bytes))
                {
                    frame.ReadStream(stream);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("NEW FRAME:");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Opcode=" + frame.Opcode);
                    Console.WriteLine("Message=" + frame.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }*/

            }
            Console.WriteLine("Started DiscordPipeServer");
        }
        long nonce = 0; 
        private async void HandleMessage(PipeFrame frame)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            if(frame.Opcode == Opcode.Handshake)
            {
                //Respond with handshake
                Console.WriteLine("Sending handshake response");
                HandshakeFrame handshakeFrame = JsonConvert.DeserializeObject<HandshakeFrame>(frame.Message);
                SetAppId?.Invoke(null, handshakeFrame.client_id);
                Ready.Data ready = new Ready.Data()
                {
                    Config = new Ready.Config()
                    {
                        APIendpoint = "//discordapp.com/api",
                        CdnHost = "cdn.discordapp.com",
                        Environment = "production"
                    },
                    User = new Ready.User()
                    {
                        Avatar = "123",
                        Bot = false,
                        Discriminator = "8813",
                        Id = "123",
                        Username = "testing"
                    },
                    Version = 1
                };
                SendFrame(Command.Dispatch, ServerEvent.Ready, ready);
            }
            else if(frame.Opcode == Opcode.Frame)
            {
                var frameMessage = JsonConvert.DeserializeObject<FrameMessage>(frame.Message);
                MessageReceived?.Invoke(null, frameMessage.Args.Activity);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        private async void SendFrame(Command command, ServerEvent e, object obj)
        {
            PipeFrame frame = new PipeFrame();
            EventPayload payload = new EventPayload(nonce);
            payload.Command = command;
            payload.Event = e;
            Ready ready = new Ready();
            payload.Data = JObject.FromObject(obj);
            using (MemoryStream stream = new MemoryStream())
            {
                //Write the stream and the send it to the pipe
                frame.Opcode = Opcode.Frame;
                frame.SetObject(payload);
                frame.WriteStream(stream);
                frame.Opcode = Opcode.Frame;
                //Get the bytes and send it
                byte[] bytes = stream.ToArray();
                await server.SendToAllAsync(bytes);
            }
            nonce++;
        }
        private void Server_ClientDisconnected(object sender, ClientConnectedArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Client disconnected");
            Console.ForegroundColor = ConsoleColor.White;
            ConnectionUpdate(null, ConnectionState.Disconnected);
        }

        private void Server_ClientConnected(object sender, ClientConnectedArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Client connected");
            Console.ForegroundColor = ConsoleColor.White;
            ConnectionUpdate(null, ConnectionState.Connected);
        }
    }
}
