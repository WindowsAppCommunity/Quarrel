// Quarrel © 2022

using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.AppConnections.Models;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace Quarrel.Services.AppConnections
{
    public class AppConnectionService
    {
        private const string RichPresenceConnectionName = "com.Quarrel.RichPresence";

        private readonly ILoggingService _loggingService;
        private readonly Dictionary<Guid, (AppConnection, BackgroundTaskDeferral)> _connections;

        public AppConnectionService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            _connections = new Dictionary<Guid, (AppConnection, BackgroundTaskDeferral)>();
        }

        public bool RegisterAppConnection(IBackgroundTaskInstance taskInstance)
        {
            var triggerDetails = (AppServiceTriggerDetails)taskInstance.TriggerDetails;
            var connection = triggerDetails.AppServiceConnection;

            _loggingService.Log(LoggedEvent.AppServiceConnectionReceived,
                (nameof(triggerDetails.Name), triggerDetails.Name),
                (nameof(triggerDetails.CallerPackageFamilyName), triggerDetails.CallerPackageFamilyName));

            AppConnection? appConnection = triggerDetails.Name switch
            {
                RichPresenceConnectionName => new RichPresenceConnection(connection),
                _ => null,
            };

            if (appConnection is null)
            {
                return false;
            }

            var deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += UnregisterService;
            _connections.Add(taskInstance.InstanceId, (appConnection, deferral));

            return true;
        }

        private void UnregisterService(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            var id = sender.InstanceId;
            var pair = _connections[id];
            pair.Item2.Complete();

            _connections.Remove(id);
        }
    }
}
