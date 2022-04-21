// Quarrel © 2022

using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.AppConnections.Models;
using System.Collections.Generic;
using Windows.ApplicationModel.AppService;

namespace Quarrel.Services.AppConnections
{
    public class AppConnectionService
    {
        private const string RichPresenceConnectionName = "Quarrel.RichPresenceAPI";

        private readonly IAnalyticsService _analyticsService;
        private readonly Dictionary<string, AppConnection> _connections;

        public AppConnectionService(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            _connections = new Dictionary<string, AppConnection>();
        }

        public bool RegisterAppConnection(AppServiceTriggerDetails connection)
        {
            _analyticsService.Log(LoggedEvent.AppServiceConnectionReceived,
                (nameof(connection.Name), connection.Name),
                (nameof(connection.CallerPackageFamilyName), connection.CallerPackageFamilyName));

            AppConnection? appConnection = connection.Name switch
            {
                RichPresenceConnectionName => new RichPresenceConnection(connection.AppServiceConnection),
                _ => null,
            };

            if (appConnection is null)
            {
                return false;
            }

            _connections.Add(appConnection.Id, appConnection);

            return true;
        }
    }
}
