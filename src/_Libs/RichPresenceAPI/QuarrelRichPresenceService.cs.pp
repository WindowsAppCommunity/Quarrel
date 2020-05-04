using Newtonsoft.Json;
using Quarrel.RichPresence.Helpers;
using Quarrel.RichPresence.Models;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace Quarrel.RichPresence
{
    public class QuarrelRichPresenceService
    {
        public QuarrelRichPresenceService(string applicationId = null)
        {

        }

        #region Events

        /// <summary>
        /// Fired when the connection to the app service closes
        /// </summary>
        public event EventHandler ConnectionClosed;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Attempts to connect to Quarrel App Service
        /// </summary>
        /// <returns>Connection Status after attempt</returns>
        public async Task<AppServiceConnectionStatus> TryConnectAsync()
        {
            // Don't bother if already connected
            if (ConnectionStatus == AppServiceConnectionStatus.Success)
                return AppServiceConnectionStatus.Success;

            _AppServiceConnection.ServiceClosed += FireConnectionClosed;
            _AppServiceConnection.AppServiceName = "Quarrel.Presence";
            _AppServiceConnection.PackageFamilyName = "38062AvishaiDernis.DiscordUWP_q72k3wbnqqnj6";
            //_AppServiceConnection.PackageFamilyName = "38062AvishaiDernis.QuarrelInsider_q72k3wbnqqnj6";

            ConnectionStatus = await _AppServiceConnection.OpenAsync();
            return ConnectionStatus ?? AppServiceConnectionStatus.Unknown;
        }

        /// <summary>
        /// Sends any Game to the Quarrel status setter
        /// </summary>
        /// <returns>Response Status</returns>
        public async Task<AppServiceResponseStatus> SetRawActivity(Game game)
        {
            ValueSet request = new ValueSet();
            request.Add(Constants.ConnectionServiceRequests.SetActivity, JsonConvert.SerializeObject(game));

            var response = await _AppServiceConnection.SendMessageAsync(request);
            return response.Status;
        }

        /// <summary>
        /// Sends request to Quarrel to set a custom status "<paramref name="status"/>"
        /// </summary>
        /// <param name="status">Custom Status string</param>
        /// <returns>Response Status</returns>
        public async Task<AppServiceResponseStatus> SetCustomActivity(string status)
        {
            Game game = new Game(status, Models.Enums.ActivityType.Custom);
            return await SetRawActivity(game);
        }

        #endregion

        #region Private

        /// <summary>
        /// Fires when service closes
        /// </summary>
        private void FireConnectionClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            ConnectionStatus = null;
            ConnectionClosed?.Invoke(sender, null);
        }

        #endregion

        #endregion

        /// <summary>
        /// Manages connection to Quarrel main app
        /// </summary>
        AppServiceConnection _AppServiceConnection = new AppServiceConnection();
        public AppServiceConnectionStatus? ConnectionStatus;
    }
}
