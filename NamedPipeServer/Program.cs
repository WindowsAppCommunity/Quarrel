using DiscordPipeImpersonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;

namespace NamedPipeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting NamedPipeServer");
            Start();
            Console.ReadKey();
        }
        static QuarrelAppService service = new QuarrelAppService();
        private static async void Start()
        {
            await service.TryConnectAsync(true);
            DiscordPipeServer server = new DiscordPipeServer();
            server.MessageReceived += Server_MessageReceived;
            server.SetAppId += Server_SetAppId;
            server.ConnectionUpdate += Server_ConnectionUpdate;
            server.Start();
        }

        static string currentAppId = null;
        private static void Server_SetAppId(object sender, string e)
        {
            currentAppId = e;
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

        }

        private static async void Server_MessageReceived(object sender, QuarrelAppService.Game e)
        {
            await service.SetActivity(e, currentAppId);
        }
    }
}
