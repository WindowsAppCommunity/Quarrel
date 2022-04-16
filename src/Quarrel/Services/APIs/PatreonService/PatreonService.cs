// Quarrel © 2022

using System.Reflection;
using System.Text.Json;
using Quarrel.Services.APIs.PatreonService.Models;

namespace Quarrel.Services.APIs.PatreonService
{
    public class PatreonService : IPatreonService
    {
        private const string ClientInfoFile = "Patreon.json";
        private readonly PatreonClientInfo _clientInfo;

        public PatreonService()
        {
            string clientInfoJson = Assembly.GetExecutingAssembly().ReadEmbeddedFile(ClientInfoFile);
            _clientInfo = JsonSerializer.Deserialize<PatreonClientInfo>(clientInfoJson);
        }
    }
}
