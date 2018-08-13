using DiscordPipeImpersonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace NamedPipeServer
{
    class Program
    {
        static AppServiceConnection connection = new AppServiceConnection();
        static void Main(string[] args)
        {
            Console.WriteLine("Starting NamedPipeServer");
            Start();
            Console.ReadKey();
        }
        private static async void Start()
        {
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;
            connection.AppServiceName = "PresenceService";
            connection.PackageFamilyName = "38062AvishaiDernis.DiscordUWP_q72k3wbnqqnj6";
            var result = await connection.OpenAsync();
            Console.WriteLine("AppService connection status: " + result.ToString());
            DiscordPipeServer server = new DiscordPipeServer();
            server.MessageReceived += Server_MessageReceived;
            server.ConnectionUpdate += Server_ConnectionUpdate;
            server.Start();

        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Console.WriteLine("App service closed");
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Console.WriteLine("App service request received");
        }

        private static async void Server_ConnectionUpdate(object sender, DiscordPipeServer.ConnectionState e)
        {
            ValueSet valueSet = new ValueSet();
            valueSet.Add("Connection", e.ToString());
            await connection.SendMessageAsync(valueSet);
            throw new NotImplementedException();
        }

        private static async void Server_MessageReceived(object sender, string e)
        {
            ValueSet valueSet = new ValueSet();
            valueSet.Add("Message", e);
            await connection.SendMessageAsync(valueSet);
            throw new NotImplementedException();
        }
    }
}
