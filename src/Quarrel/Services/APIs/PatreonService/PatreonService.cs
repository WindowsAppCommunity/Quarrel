// Quarrel © 2022

using System.Reflection;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Quarrel.Services.Analytics;
using Quarrel.Services.APIs.PatreonService.Models;
using Quarrel.Services.Analytics.Enums;

namespace Quarrel.Services.APIs.PatreonService
{
    public class PatreonService : IPatreonService
    {
        private const string ClientInfoFile = "Patreon.json";
        private readonly IAnalyticsService _analyticsService;
        private readonly PatreonClientInfo _clientInfo;

        public PatreonService(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;

            string clientInfoJson = Assembly.GetExecutingAssembly().ReadEmbeddedFile(ClientInfoFile);
            PatreonClientInfo? clientInfo = JsonSerializer.Deserialize<PatreonClientInfo>(clientInfoJson);
            if (clientInfo is null)
            {
                _analyticsService.Log(LoggedEvent.PatreonClientInfoNotFound);
                Guard.IsNotNull(clientInfo);
            }

            _clientInfo = clientInfo;
        }
    }
}
