using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Discord_UWP.Managers
{
    public static class RPCServer
    {
        static bool IsListening = false;
        static StreamSocketListener listener = new StreamSocketListener();
        public static async Task<bool> TryStart()
        {
            //The server should run on local host, on the first functional port from 6463 to 6472
            listener.ConnectionReceived += Listener_ConnectionReceived;
            for (int port = 6463; port <= 6472 && !IsListening; port++)
                await StartListener(port);

            

            if (IsListening)
            {
                //Server is listening
                return true;
            }
            else
            {
                //Server is not listening
                return false;
            }
        }

        private static async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string request;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }
            Debug.WriteLine(request);
        }

        private async static void Listener_Request(object sender, HttpListenerRequestEventArgs e)
        {
            string content = await e.Request.ReadContentAsStringAsync();
            Debug.WriteLine(content);
        }

        private static async Task StartListener(int port)
        {
            try
            {
                await listener.BindServiceNameAsync(port.ToString());
                IsListening = true;
            } catch {}
            
        }
    }
}
