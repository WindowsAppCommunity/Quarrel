using System;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Services.Discord.Rest;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace Quarrel
{
    /// <summary>
    /// Section of App that handles the AppServiceConnection used for Rich Presence
    /// </summary>
    public partial class App
    {
        #region Events

        public event EventHandler ConnectedToAppService;

        #endregion

        #region Methods

        /// <summary>
        /// Handles AppServiceConnection opening
        /// </summary>
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            IBackgroundTaskInstance taskInstance = args.TaskInstance;
            AppServiceTriggerDetails appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _appServiceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;
            _appServiceConnection = appService.AppServiceConnection;
            _appServiceConnection.RequestReceived += HandleServiceRequest;
            ConnectedToAppService?.Invoke(null, null);
        }

        /// <summary>
        /// Handles Request
        /// </summary>
        private void HandleServiceRequest(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (SimpleIoc.Default.ContainsCreated<IDiscordService>())
            {
                var game = JsonConvert.DeserializeObject<Game>(args.Request.Message[Constants.ConnectionServiceRequests.SetActivity].ToString());
                SimpleIoc.Default.GetInstance<IDiscordService>().Gateway.Gateway.UpdateStatus(game);
            }
        }

        /// <summary>
        /// Handles AppServiceConnection closing
        /// </summary>
        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _appServiceDeferral.Complete();
        }

        #endregion

        #region Properties 


        public AppServiceConnection _appServiceConnection;
        public BackgroundTaskDeferral _appServiceDeferral;

        #endregion
    }
}
