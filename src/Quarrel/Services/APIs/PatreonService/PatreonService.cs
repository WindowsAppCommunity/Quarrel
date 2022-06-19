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
        private readonly ILoggingService _loggingService;
        private readonly PatreonClientInfo _clientInfo;

        public PatreonService(ILoggingService loggingService)
        {
            _loggingService = loggingService;

            string clientInfoJson = Assembly.GetExecutingAssembly().ReadEmbeddedFile(ClientInfoFile);
            PatreonClientInfo? clientInfo = JsonSerializer.Deserialize<PatreonClientInfo>(clientInfoJson);
            if (clientInfo is null)
            {
                _loggingService.Log(LoggedEvent.PatreonClientInfoNotFound);
            }

            Guard.IsNotNull(clientInfo);
            _clientInfo = clientInfo;
        }
    }
}
