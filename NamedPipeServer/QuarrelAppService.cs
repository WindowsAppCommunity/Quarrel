using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System.Diagnostics;
using Newtonsoft.Json;

namespace NamedPipeServer
{
    class QuarrelAppService
    {
        private class NullApplicationIdException : Exception
        {
            public NullApplicationIdException() { }
            public NullApplicationIdException(string message) : base(message)  { }
            public NullApplicationIdException(string message, Exception inner) : base(message, inner)   { }
        }
        /// <summary>
        /// Fired when the connection to the app service closes
        /// </summary>
        public event EventHandler ConnectionClosed;
        /// <summary>
        /// Fired when the connection to the app service is opened
        /// </summary>
        public event EventHandler ConnectionOpened;
        public enum ActivityType { Playing, Listening, Watching }
        public class Activity
        {
            [JsonProperty("state")]
            public string State { get; set; }
            [JsonProperty("details")]
            public string Details { get; set; }
            [JsonProperty("timestamps")]
            public TimestampsClass Timestamps { get; set; }
            [JsonProperty("assets")]
            public AssetClass Assets { get; set; }
            [JsonProperty("party")]
            public PartyClass Party { get; set; }
            public class TimestampsClass
            {
                [JsonProperty("start")]
                public DateTimeOffset Start { get; set; }
                [JsonProperty("end")]
                public DateTimeOffset End { get; set; }
            }
            public class AssetClass
            {
                [JsonProperty("large_text")]
                public string LargeImageText { get; set; }
                [JsonProperty("large_image")]
                public string LargeImageKey { get; set; }
                [JsonProperty("small_text")]
                public string SmallImageText { get; set; }
                [JsonProperty("small_image")]
                public string SmallImageKey { get; set; }
            }
            public class PartyClass
            {
                public string PartyId { get; set; }
                public int PartySize { get; set; }
                public int PartyMax { get; set; }
                public string SpectateSecret { get; set; }
                public string JoinSecret { get; set; }
            }
        }
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
        /// <param name="connectIfClosed">Connect to Quarrel when it is opened, if it is currently closed</param>
        /// <returns></returns>
        public async Task TryConnectAsync(bool connectIfClosed)
        {
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;
            connection.AppServiceName = "PresenceService";
            connection.PackageFamilyName = "38062AvishaiDernis.DiscordUWP_q72k3wbnqqnj6";
          
            Status = await connection.OpenAsync();
            Console.WriteLine("AppService connection status=" + Status);
        }

        private string tempActivity = null;
        /// <summary>
        /// Set the current activity through a <see cref="Activity"/> object
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="applicationId">If the application ID was not specified during initialization, it MUST be specified here</param>
        /// <returns></returns>
        public async Task<bool> SetActivity(Activity activity, string applicationId = null)
        {
            if(ApplicationId == null)
            {
                if (applicationId == null) throw new NullApplicationIdException("The Application ID has not been specified during initialization, nor during this function call");
                else ApplicationId = applicationId;
            }
            //Conver the activity to a valid JSON request
            ValueSet valueset = new ValueSet();
            valueset.Add("SET_ACTIVITY", "");
            var response = connection.SendMessageAsync(valueset);
            return false;
        }

        /// <summary>
        /// Set the current activity directly with a JSON string (not recommended)
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public async Task<bool> SetActivity(string activity)
        {
            return false;
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
