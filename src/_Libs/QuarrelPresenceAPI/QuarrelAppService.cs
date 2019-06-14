using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System.Diagnostics;

namespace Quarrel.RichPresence
{
    public class QuarrelAppService
    {
        private class NullApplicationIdException : Exception
        {
            public NullApplicationIdException() { }
            public NullApplicationIdException(string message) : base(message) { }
            public NullApplicationIdException(string message, Exception inner) : base(message, inner) { }
        }
        /// <summary>
        /// Fired when the connection to the app service closes
        /// </summary>
        public event EventHandler ConnectionClosed;
        /// <summary>
        /// Fired when the connection to the app service is opened
        /// </summary>
        //public event EventHandler ConnectionOpened;

        
        private uint pid = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId;
        private string ApplicationId;

        public QuarrelAppService(string applicationId = null)
        {
            ApplicationId = applicationId;
        }

        AppServiceConnection connection = new AppServiceConnection();
        public AppServiceConnectionStatus Status;

        /// <summary>
        /// Try connecting to Quarrel
        /// </summary>
        /// <returns></returns>
        public async Task<AppServiceConnectionStatus> TryConnectAsync()
        {
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;
            connection.AppServiceName = "Quarrel.Presence";
            connection.PackageFamilyName = "38062AvishaiDernis.DiscordUWP_q72k3wbnqqnj6";

            Status = await connection.OpenAsync();
            return Status;
            //Console.WriteLine("AppService connection status=" + Status);
        }
        
        /// <summary>
        /// Set the current activity through a <see cref="Activity"/> object
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="applicationId">If the application ID was not specified during initialization, it MUST be specified here</param>
        /// <returns>The response status of the sent Message</returns>
        public async Task<AppServiceResponseStatus> SetActivity(GameBase activity, string applicationId = null)
        {
            if (ApplicationId == null)
            {
                if (applicationId == null) throw new NullApplicationIdException("The Application ID has not been specified during initialization, nor during this function call");
                else ApplicationId = applicationId;
            }

            //Conver the activity to a valid JSON request
            ValueSet valueset = new ValueSet();
            valueset.Add("SET_ACTIVITY", JsonConvert.SerializeObject(activity));
            var response = await connection.SendMessageAsync(valueset);
            return response.Status;
        }

        public async Task<AppServiceResponseStatus> ClearActivity()
        {
            ValueSet valueset = new ValueSet();
            valueset.Add("SET_ACTIVITY", "");
            var response = await connection.SendMessageAsync(valueset);
            return response.Status;
        }

        public void CloseAppServiceConnection()
        {
            connection.Dispose();
        }

        /// <summary>
        /// Set the current activity directly with a JSON string (not recommended)
        /// </summary>
        /// <param name="activity"></param>
        /// <returns>The response status of the sent Message</returns>
        public async Task<AppServiceResponseStatus> SetActivity(string activity)
        {
            ValueSet valueset = new ValueSet();
            valueset.Add("SET_ACTIVITY", activity);
            var response = await connection.SendMessageAsync(valueset);

            return response.Status;
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            ConnectionClosed?.Invoke(sender, null);
        }
        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Console.WriteLine("RECEIVED CONNECTION REQUEST");
        }
    }
}
